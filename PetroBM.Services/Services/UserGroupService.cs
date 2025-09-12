using PetroBM.Common.Util;
using PetroBM.Data;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using log4net;

namespace PetroBM.Services.Services
{
    public interface IUserGroupService
    {
        IList<MUserGrpPermission> InitUserGrpPermissionList();

        IEnumerable<MUserGrp> GetAllUserGrp();
		bool UpdateUserGrpPermission(int userGrpId);
		MUserGrp GetUserGrp(int userGrpId);
        bool CreateUserGrp(MUserGrp userGrp, IList<MUserGrpPermission> userGrpPermissionList);
        bool UpdateUserGrp(MUserGrp userGrp, IList<MUserGrpPermission> userGrpPermissionList);
        bool DeleteUserGrp(MUserGrp userGrp);
        bool UpdateUserGroupUser_By_ListUserGroupId(MUser user, int[] lstusergroupId);
        int[] Get_ListUseGroupId_By_UserName(string user);


    }

    public class UserGroupService : IUserGroupService
    {
        ILog log = log4net.LogManager.GetLogger(typeof(UserGroupService));

        private readonly IUserGroupRepository UserGrpRepository;
        private readonly IPermissionRepository PermissionRepository;
        private readonly IUserGrpPermissionRepository UserGrpPermissionRepository;
        private readonly IEventService EventService;
        private readonly IUnitOfWork unitOfWork;

        public UserGroupService(IUserGroupRepository UserGrpRepository, IPermissionRepository PermissionRepository,
            IUserGrpPermissionRepository UserGrpPermissionRepository, IEventService eventService, IUnitOfWork unitOfWork)
        {
            this.UserGrpRepository = UserGrpRepository;
            this.PermissionRepository = PermissionRepository;
            this.UserGrpPermissionRepository = UserGrpPermissionRepository;
            this.EventService = eventService;
            this.unitOfWork = unitOfWork;
        }

        #region IUserGroupService Members

        public IEnumerable<MPermission> GetAllPermission()
        {
            return PermissionRepository.GetAll().OrderBy(per => per.PermissionCode);
        }

        public IList<MUserGrpPermission> InitUserGrpPermissionList()
        {
            var permissionList = GetAllPermission().OrderBy(x=>x.PermissionCode);
            var userGrpPermissionList = new List<MUserGrpPermission>();
            foreach (var per in permissionList)
            {
                var perUserGrp = new MUserGrpPermission();
                perUserGrp.MPermission = per;
                perUserGrp.PermissionCode = per.PermissionCode;
                perUserGrp.ActiveFlg = Constants.FLAG_OFF;
                userGrpPermissionList.Add(perUserGrp);
            }
            return userGrpPermissionList;
        }

        public IEnumerable<MUserGrp> GetAllUserGrp()
        {
            return UserGrpRepository.GetAll().Where(grp => grp.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(grp => grp.GrpId);
        }

        public MUserGrp GetUserGrp(int userGrpId)
        {
			return UserGrpRepository.GetById(userGrpId);
		}

		public bool UpdateUserGrpPermission(int userGrpId)
		{
			bool rs = true; 
			MUserGrp userGrp = UserGrpRepository.GetById(userGrpId);
			var userGrpPermissionNotExistedList = InitUserGrpPermissionList()
				.Where(up => !userGrp.MUserGrpPermissions.Any(up2 => up2.PermissionCode == up.PermissionCode));

			if (userGrpPermissionNotExistedList.Count() > 0)
			{
				try
				{
					using (TransactionScope ts = new TransactionScope())
					{
						foreach (var per in userGrpPermissionNotExistedList)
						{
							per.GrpId = userGrp.GrpId;
							UserGrpPermissionRepository.Add(per);
						}
						SaveUserGrp();
						ts.Complete();
					}
				}
				catch (Exception ex)
				{
					rs = false;
					log.Error(ex);
				}
			}			

			return rs;
		}

		public bool CreateUserGrp(MUserGrp userGrp, IList<MUserGrpPermission> userGrpPermissionList)
        {
            var rs = false;

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    UserGrpRepository.Add(userGrp);
                    SaveUserGrp();

                    foreach (var per in userGrpPermissionList)
                    {
                        per.GrpId = userGrp.GrpId;
                        UserGrpPermissionRepository.Add(per);
                    }
                    SaveUserGrp();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                EventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_MANAGER_USERGROUP_CREATE, userGrp.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool UpdateUserGrp(MUserGrp userGrp, IList<MUserGrpPermission> userGrpPermissionList)
        {
            var rs = false;

            try
            {
                foreach (var per in userGrp.MUserGrpPermissions)
                {
                    foreach (var p in userGrpPermissionList)
                    {
                        if (per.PermissionCode == p.PermissionCode)
                        {
                            per.ActiveFlg = p.ActiveFlg;
                        }
                    }
                }

                UserGrpRepository.Update(userGrp);
                SaveUserGrp();
                rs = true;

                // Log event
                EventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_MANAGER_USERGROUP_UPDATE, userGrp.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool DeleteUserGrp(MUserGrp userGrp)
        {
            var rs = false;

            try
            {
                // Dont delete admin group
                if (userGrp.GrpId > Constants.ADMIN_GROUP_ID)
                {
                    //foreach (var user in userGrp.MUsers)
                    //{
                    //    user.GrpId = null;
                    //}

                    userGrp.DeleteFlg = Constants.FLAG_ON;
                    UserGrpRepository.Update(userGrp);
                    SaveUserGrp();
                    rs = true;

                    // Log event
                    EventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                        Constants.EVENT_MANAGER_USERGROUP_DELETE, userGrp.UpdateUser);
                }
            }
            catch { }

            return rs;
        }

        private void SaveUserGrp()
        {
            unitOfWork.Commit();
        }

        public bool UpdateUserGroupUser_By_ListUserGroupId(MUser user, int[] lstusergroupId)
        {
            var rs = false;
            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Delete_UserGroupUser_By_UserName";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("UserName", user.UserName);
                            cmd.Parameters.Add(param);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            for (int i = 0; i < lstusergroupId.Count(); i++)
                            {
                                cmd.CommandText = "Update_UserGroupUser_By_UserName_UserGrpId";
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter param4 = new SqlParameter("UserName", user.UserName);
                                SqlParameter param5 = new SqlParameter("UserGrpId", lstusergroupId[i]);
                                SqlParameter param6 = new SqlParameter("User", user.UpdateUser);
                                cmd.Parameters.Add(param4);
                                cmd.Parameters.Add(param5);
                                cmd.Parameters.Add(param6);
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                            rs = true;
                        }
                        conn.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public int[] Get_ListUseGroupId_By_UserName(string user)
        {
            var listUseGroupId = new List<int>();
            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Select_ListUserGroupId_By_UserName";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("UserName", user);
                            cmd.Parameters.Add(param);
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                listUseGroupId.Add(int.Parse(reader["GrpId"].ToString()));
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

            return listUseGroupId.ToArray();
        }
        #endregion
    }
}
