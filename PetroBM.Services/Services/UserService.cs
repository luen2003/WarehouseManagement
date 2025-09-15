using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using PetroBM.Domain.Entities;
using PetroBM.Data.Repositories;
using PetroBM.Data.Infrastructure;
using PetroBM.Common.Util;
using System.Web.Security;
using System.Threading;
using System.Data;
using PetroBM.Data;
using System.Data.SqlClient;
using log4net;

namespace PetroBM.Services.Services
{
	public interface IUserService
	{
		IEnumerable<MUser> GetAllUser();
		MUser GetUser(String userName);
		bool CheckPermission(String userName, int permissionCode, bool flag);
		bool Login(string userName, string password);
		void Logout(string userName);
		bool CheckPass(string userName, string password);
		bool ChangePass(string userName, string password, string newPass);
		bool CreateUser(MUser user);
		bool UpdateUser(MUser user);
		bool ChangePass(MUser user);
		bool DeleteUser(string userName);
		bool SaveSelectedField(string userName, string value);
		bool SelectedConfigArmFields(string userName, string value);
		bool CheckPermission_ManagerReportChart(String userName, int permissionCode, bool flag);
		bool CheckPermission_General(String userName, int permissionCode, bool flag);

		List<MPermission> GetListPermission_ByUserName(String userName, int fromPermissionCode, int toPermissionCode);

		List<MWareHousePermission> GetListWareHousePermission_ByUserName(String userName, int fromPermissionCode, int toPermissionCode);
		int GetJobTitlesByUserName(string userName);

		string GetUserIDByUserName(string userName);

		string GetSerialNumberByUserName(string userName);


    }

	public class UserService : IUserService
	{
		ILog log = log4net.LogManager.GetLogger(typeof(UserService));

		private readonly IUserRepository userRepository;
		private readonly IEventService eventService;
		private readonly IUnitOfWork unitOfWork;

		public UserService(IUserRepository userRepository, IEventService eventService, IUnitOfWork unitOfWork)
		{
			this.userRepository = userRepository;
			this.eventService = eventService;
			this.unitOfWork = unitOfWork;
		}

		#region UserService Members
		public int GetJobTitlesByUserName(string userName)
		{
			try
			{
				var user = this.GetUser(userName);
				if (user != null && user.DeleteFlg == Constants.FLAG_OFF)
				{
					return user.JobTitles;
				}
			}
			catch (Exception ex)
			{
				log.Error(ex);
			}
			return 0;
		}

		public string GetUserIDByUserName(string userName)
		{
			try
			{
				var user = this.GetUser(userName);
				if (user != null && user.DeleteFlg == Constants.FLAG_OFF)
				{
					return user.UserID;
				}
			}
			catch (Exception ex)
			{
				log.Error(ex);
			}
			return string.Empty;
        }

		public string GetSerialNumberByUserName(string userName)
		{
			try
			{
				var user = this.GetUser(userName);
				if (user != null && user.DeleteFlg == Constants.FLAG_OFF)
				{
					return user.SerialNumber;
				}
			}
			catch (Exception ex)
			{
				log.Error(ex);
			}
			return string.Empty;
        }

        public IEnumerable<MUser> GetAllUser()
		{
			return userRepository.GetAll().Where(user => user.DeleteFlg == Constants.FLAG_OFF)
				.OrderBy(user => user.InsertDate);
		}

		public MUser GetUser(String userName)
		{
			return userRepository.Get(user => (user.DeleteFlg == Constants.FLAG_OFF) && (user.UserName.Equals(userName)));
		}

		public bool CheckPermission(String userName, int permissionCode, bool flag)
		{
			bool rs = false;

			try
			{
				//if (flag)
				//{
				//    int[] list = { 4, 6, 7 };

				//    if (list.Contains(permissionCode))
				//    {
				//        var date = new DateTime(2018, 5, 1);

				//        if (date < DateTime.Now)
				//        {
				//            var time = (DateTime.Now - date).TotalDays;
				//            Thread.Sleep((int)time * 2000);
				//        }
				//    }
				//}

				//ThangNK comment tạm sử lí sau
				//var user = GetUser(userName);
				//var permission = user.MUserGrp.MUserGrpPermissions.Where(ugp => ugp.PermissionCode == permissionCode).FirstOrDefault();
				//rs = permission.ActiveFlg;
				// Đặt tạm
				rs = true;

			}
			catch (Exception ex)
			{
				log.Error(ex);
			}

			return rs;
		}

		private void SaveUser()
		{
			unitOfWork.Commit();
		}

		public bool Login(string userName, string password)
		{
			bool rs = false;

			if (String.IsNullOrEmpty(password))
			{
				return rs;
			}

			password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, Constants.PASS_FORMAT);
			var user = userRepository.getUserLogged(userName, password);

			if (user != null)
			{
				rs = true;

				eventService.CreateEvent(Constants.EVENT_TYPE_AUTHENTICATION,
				   Constants.EVENT_LOGIN, userName);
			}

			return rs;
		}

		public void Logout(string userName)
		{
			eventService.CreateEvent(Constants.EVENT_TYPE_AUTHENTICATION,
				   Constants.EVENT_LOGOUT, userName);
		}

		public bool CheckPass(string userName, string password)
		{
			bool rs = false;

			if (String.IsNullOrEmpty(password))
			{
				return rs;
			}

			password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, Constants.PASS_FORMAT);

			var user = userRepository.getUserLogged(userName, password);
			if (user != null)
			{
				rs = true;
			}

			return rs;
		}

		public bool ChangePass(string userName, string password, string newPass)
		{
			bool rs = false;

			if (String.IsNullOrEmpty(password))
			{
				return rs;
			}

			password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, Constants.PASS_FORMAT);
			var user = userRepository.getUserLogged(userName, password);

			if (user == null)
			{
				rs = false;
			}
			else
			{
				newPass = FormsAuthentication.HashPasswordForStoringInConfigFile(newPass, Constants.PASS_FORMAT);
				user.Passwd = newPass;
				userRepository.Update(user);
				unitOfWork.Commit();
				rs = true;
			}
			return rs;
		}

		public bool UpdateUser(MUser user)
		{
			var rs = false;
			try
			{
				using (TransactionScope ts = new TransactionScope())
				{
					userRepository.Update(user);
					unitOfWork.Commit();
					ts.Complete();
					rs = true;
				}

				eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
						Constants.EVENT_MANAGER_USER_UPDATE, user.UpdateUser);
			}
			catch (Exception ex)
			{
				log.Error(ex);
			}

			return rs;
		}

		public bool ChangePass(MUser user)
		{
			var rs = false;

			user.Passwd = FormsAuthentication.HashPasswordForStoringInConfigFile(user.Passwd, Constants.PASS_FORMAT);
			rs = UpdateUser(user);

			return rs;
		}

		public bool DeleteUser(string userName)
		{
			MUser user = this.GetUser(userName);
			var rs = false;
			try
			{
				using (TransactionScope ts = new TransactionScope())
				{
					user.DeleteFlg = Constants.FLAG_ON;
					userRepository.Update(user);

					unitOfWork.Commit();
					ts.Complete();
					rs = true;
				}

				eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
					Constants.EVENT_MANAGER_USER_DELETE, user.UpdateUser);
			}
			catch (Exception e)
			{
				log.Error(e);
			}

			return rs;
		}

		public bool CreateUser(MUser user)
		{
			var rs = false;
			try
			{
				using (TransactionScope ts = new TransactionScope())
				{
					user.Passwd = FormsAuthentication.HashPasswordForStoringInConfigFile(user.Passwd, Constants.PASS_FORMAT);
					userRepository.Add(user);
					unitOfWork.Commit();
					ts.Complete();
					rs = true;
				}

				eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
					   Constants.EVENT_MANAGER_USER_CREATE, user.InsertUser);
			}
			catch (Exception ex)
			{
				log.Error(ex);
			}

			return rs;
		}

		public bool SaveSelectedField(string userName, string value)
		{
			var rs = false;
			var user = this.GetUser(userName);

			if (user == null)
			{
				rs = false;
			}
			else
			{
				user.SelectedFields = value;
				userRepository.Update(user);
				unitOfWork.Commit();
				rs = true;
			}


			eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
				   Constants.EVENT_CONFIG_SELECTED_FIELD, user.InsertUser);

			return rs;
		}


		public bool SelectedConfigArmFields(string userName, string value)
		{
			var rs = false;
			var user = this.GetUser(userName);

			if (user == null)
			{
				rs = false;
			}
			else
			{
				user.SelectedConfigArmFields = value;
				userRepository.Update(user);
				unitOfWork.Commit();
				rs = true;
			}

			eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
				   Constants.EVENT_CONFIG_SELECTED_FIELD, user.InsertUser);

			return rs;
		}
		/// <summary>
		/// Chỉ check permisstion cho Báo cáo, Đồ thị, Quản trị hệ thống
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="permissionCode"></param>
		/// <param name="flag"></param>
		/// <returns></returns>
		public bool CheckPermission_ManagerReportChart(string userName, int permissionCode, bool flag)
		{
			var chk = false;
			try
			{
				using (var context = new PetroBMContext())
				{
					using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
					{
						conn.Open();
						using (var cmd = conn.CreateCommand())
						{
							cmd.CommandText = "Select_Permission_By_ManagerReportChart";
							cmd.CommandType = CommandType.StoredProcedure;
							SqlParameter param = new SqlParameter("UserName", userName);
							SqlParameter param2 = new SqlParameter("PermissionCode", permissionCode);
							cmd.Parameters.Add(param);
							cmd.Parameters.Add(param2);
							SqlDataReader reader = cmd.ExecuteReader();
							while (reader.Read())
							{
								chk = bool.Parse(reader["CheckPermission"].ToString());
							}
						}
						conn.Close();
					}
				}

			}
			catch (Exception ex)
			{
				log.Error(ex);
			}

			return chk;
		}

		public bool CheckPermission_General(string userName, int permissionCode, bool flag)
		{
			var chk = false;
			try
			{
				using (var context = new PetroBMContext())
				{
					using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
					{
						conn.Open();
						using (var cmd = conn.CreateCommand())
						{
							cmd.CommandText = "Select_Permission_By_ManagerReportChart";
							cmd.CommandType = CommandType.StoredProcedure;
							SqlParameter param = new SqlParameter("UserName", userName);
							SqlParameter param2 = new SqlParameter("PermissionCode", permissionCode);
							cmd.Parameters.Add(param);
							cmd.Parameters.Add(param2);
							SqlDataReader reader = cmd.ExecuteReader();
							while (reader.Read())
							{
								chk = bool.Parse(reader["CheckPermission"].ToString());
							}
						}
						conn.Close();
					}
				}

			}
			catch (Exception ex)
			{
				log.Error(ex);
			}

			return chk;
		}

		public List<MPermission> GetListPermission_ByUserName(string userName, int fromPermission, int toPermission)
		{
			var lst = new List<MPermission>();
			try
			{
				using (var context = new PetroBMContext())
				{
					using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
					{
						conn.Open();
						using (var cmd = conn.CreateCommand())
						{
							cmd.CommandText = "Select_Permission_By_UserName_FromPermission_ToPermission";
							cmd.CommandType = CommandType.StoredProcedure;
							SqlParameter param = new SqlParameter("UserName", userName);
							SqlParameter param2 = new SqlParameter("FromPermissionCode", fromPermission);
							SqlParameter param3 = new SqlParameter("ToPermissionCode", toPermission);
							cmd.Parameters.Add(param);
							cmd.Parameters.Add(param2);
							cmd.Parameters.Add(param3);
							SqlDataReader reader = cmd.ExecuteReader();
							while (reader.Read())
							{
								var it = new MPermission();
								it.PermissionCode = int.Parse(reader["PermissionCode"].ToString());
								it.Type = bool.Parse(reader["Type"].ToString());
								it.Name = reader["Name"].ToString();
								it.Description = reader["Description"].ToString();
								lst.Add(it);
							}
						}
						conn.Close();
					}
				}

			}
			catch (Exception ex)
			{
				log.Error(ex);
			}


			return lst;
		}

		public List<MWareHousePermission> GetListWareHousePermission_ByUserName(string userName, int fromPermissionCode, int toPermissionCode)
		{
			var lst = new List<MWareHousePermission>();
			try
			{
				using (var context = new PetroBMContext())
				{
					using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
					{
						conn.Open();
						using (var cmd = conn.CreateCommand())
						{
							cmd.CommandText = "Select_GetWareHouse_ByRole_UserName_FromPermission_ToPermission";
							cmd.CommandType = CommandType.StoredProcedure;
							SqlParameter param = new SqlParameter("UserName", userName);
							SqlParameter param2 = new SqlParameter("FromPermissionCode", fromPermissionCode);
							SqlParameter param3 = new SqlParameter("ToPermissionCode", toPermissionCode);
							cmd.Parameters.Add(param);
							cmd.Parameters.Add(param2);
							cmd.Parameters.Add(param3);
							SqlDataReader reader = cmd.ExecuteReader();
							while (reader.Read())
							{
								var it = new MWareHousePermission();
								it.PermissionCode = int.Parse(reader["PermissionCode"].ToString());
								it.WareHouseCode = byte.Parse(reader["WareHouseCode"].ToString());
								lst.Add(it);
							}
						}
						conn.Close();
					}
				}

			}
			catch (Exception ex)
			{
				log.Error(ex);
			}


			return lst;
		}

		#endregion
	}
}
