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
using System.ComponentModel.Design;

namespace PetroBM.Services.Services
{
    public interface ICommandService
    {
        IEnumerable<MCommand> GetAllCommand();
        IEnumerable<MCommand> GetAllCommandOrderByName();
        IEnumerable<MCommand> GetAllCommandByname(string cardData);
        IEnumerable<MCommand> GetAllCommandByCommandID(int commandId);

        int getCertificateNumber();
        
        IEnumerable<MCommand> GetAllCommandByCardSerial(long cardSerial);
        bool CreateCommand(MCommand command);
        bool UpdateCommand(MCommand command);
        bool DeleteCommand(int id);

        MCommand FindCommandById(int id);
        int GetNewId();
        decimal? GetWorkOrderMax();

        bool UpdateCommandChangeDate(int commandId,DateTime timeorder, String user);
        bool CancelCommand(int commandId, String user);
		bool ApproveCommand(int commandId, String user);
		bool CancelCommandDetail(int id, String user);
        bool UnApproveCommand(int commandId, String user);

        bool UpdateFlag(int Id, String user);

        bool CheckCommandID(int commanddetail);
        List<MCommand> GetList_Command_By_Vehicle(byte? wareHouseCode, long? workOrder, string cardSerial, string cardData, DateTime? fromDate, DateTime? toDate, string vehicle,int? flag, string Controller); 
        bool UpdateCustomerCommandID(int commandId, string CustomerCode , string user);
    }

    public class CommandService : ICommandService
    {

        ILog log = log4net.LogManager.GetLogger(typeof(CommandService));
        private readonly ICommandRepository commandRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public CommandService(ICommandRepository commandRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.commandRepository = commandRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateCommand(MCommand command)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    commandRepository.Add(command);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                //eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                //    Constants.EVENT_CONFIG_COMMAND_CREATE, command.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool DeleteCommand(int id)
        {
            MCommand command = this.FindCommandById(id);
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    command.DeleteFlg = Constants.FLAG_ON;
                    commandRepository.Update(command);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_COMMAND_DELETE, command.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public IEnumerable<MCommand> GetAllCommand()
        {
            return commandRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(p => p.InsertDate);
        }

        public IEnumerable<MCommand> GetAllCommandOrderByName()
        {
            return commandRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(p => p.CardData);
        }

        public bool UpdateCommand(MCommand command)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    commandRepository.Update(command);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_COMMAND_UPDATE, command.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public MCommand FindCommandById(int id)
        {
            return commandRepository.GetById(id);
        }
        public IEnumerable<MCommand> GetAllCommandByname(string cardData)
        {

            return commandRepository.GetAll().Where(p => p.CardData == cardData).OrderBy(p => p.CommandID);
        } 
        public IEnumerable<MCommand> GetAllCommandByCardSerial(long cardSerial)
        {
            return commandRepository.GetAll().Where(p => p.CardSerial == cardSerial).OrderBy(p => p.CommandID);
        }
        public IEnumerable<MCommand> GetAllCommandByCommandID(int commandID)
        {
            return commandRepository.GetAll().Where(p => p.CommandID == commandID).OrderBy(p => p.CommandID);
        }
        public int GetNewId()
        {
            return commandRepository.GetAll().Max(x => x.CommandID);
        }

        public decimal?  GetWorkOrderMax()
        {
            var commandIdMax = commandRepository.GetAll().Max(x => x.CommandID);

            return commandRepository.GetById(commandIdMax).WorkOrder;
        }
		/// <summary>
		/// Tìm kiếm Lệnh theo xe
		/// </summary>
		/// <param name="wareHouseCode">Mã kho</param>
		/// <param name="workOrder"> Lệnh</param>
		/// <param name="cardSerial"> Card serial</param>
		/// <param name="cardData">Card Data</param>
		/// <param name="fromDate">Từ ngày</param>
		/// <param name="toDate">Đến ngày</param>
		/// <param name="vehicle">Phương tiện</param>
		/// <returns></returns>
		public List<MCommand> GetList_Command_By_Vehicle(byte? wareHouseCode, long? certificateNumber, string cardSerial, string cardData, DateTime? fromDate, DateTime? toDate, string vehicle,int? flag, string Controller)
        {
            List<MCommand> lst = new List<MCommand>();

            try
            {

                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            if (Controller != null && Controller == "Bill")
                                cmd.CommandText = "SelectList_Bills_By_Vehicle";
                            else
                                cmd.CommandText = "SelectList_Command_By_Vehicle";
                            
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param1 = new SqlParameter("WareHouseCode", wareHouseCode?? 0);
                            SqlParameter param2 = new SqlParameter("CertificateNumber", certificateNumber ?? 0);
                            SqlParameter param3 = new SqlParameter("CardSerial", cardSerial?? "");
                            SqlParameter param4 = new SqlParameter("CardData", cardData?? "");
                            SqlParameter param5 = new SqlParameter("FromDate", fromDate);
                            SqlParameter param6 = new SqlParameter("ToDate", toDate);
                            SqlParameter param7 = new SqlParameter("Vehicle", vehicle?? "");
                            SqlParameter param8 = new SqlParameter("Flag", flag ?? 100);
                            
                            cmd.Parameters.Add(param1);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.Parameters.Add(param4);
                            cmd.Parameters.Add(param5);
                            cmd.Parameters.Add(param6);
                            cmd.Parameters.Add(param7);
                            cmd.Parameters.Add(param8);
                            SqlDataReader reader = cmd.ExecuteReader();
                            
                            while (reader.Read())
                            {
                                var it = new MCommand();
                                it.CommandID = int.Parse(reader["CommandID"].ToString());
                                it.WorkOrder = decimal.Parse(reader["WorkOrder"].ToString());
                                it.CustomerCode = reader["CustomerCode"].ToString();
                                it.CardData = reader["CardData"].ToString();
                                //it.CardSerial = long.Parse(reader["CardSerial"].ToString());
                                it.CardSerial = !string.IsNullOrEmpty(reader["CardSerial"].ToString())
                                ? long.Parse(reader["CardSerial"].ToString().Replace(",", ""))
                                : (long?)null;
                                it.TimeOrder = DateTime.Parse(reader["TimeOrder"].ToString());
                                it.VehicleNumber = reader["VehicleNumber"].ToString();
                                it.CertificateNumber = int.Parse(reader["CertificateNumber"].ToString());
                                it.DriverName = reader["DriverName"].ToString();
								it.Status = decimal.Parse(reader["Status"].ToString());
                                it.IsApprove = int.Parse(reader["IsApprove"].ToString());
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

           // throw new NotImplementedException();
        }

        public bool UpdateCommandChangeDate(int commandId, DateTime timeorder, String user)
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
                            cmd.CommandText = "Update_Command_By_ChangeDate";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param1 = new SqlParameter("CommandID", commandId);
                            SqlParameter param2 = new SqlParameter("TimeOrder", timeorder);
                            cmd.Parameters.Add(param1);
                            cmd.Parameters.Add(param2);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }

				// Log event
				eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
				   Constants.EVENT_CONFIG_COMMAND_MOVE, user);
			}
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool CancelCommand(int commandId, String user)
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
                            cmd.CommandText = "Update_Command_By_CancelCommand";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param1 = new SqlParameter("CommandID", commandId);
                            cmd.Parameters.Add(param1);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }

				// Log event
				eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
				   Constants.EVENT_CONFIG_COMMAND_CANCEL, user);
			}
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

		public bool ApproveCommand(int commandId,String user)
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
							cmd.CommandText = "Update_Command_By_ApproveCommand";
							cmd.CommandType = CommandType.StoredProcedure;
							SqlParameter param1 = new SqlParameter("CommandID", commandId);
							cmd.Parameters.Add(param1);
							cmd.ExecuteNonQuery();
							rs = true;
						}
						conn.Close();
					}
				}

				// Log event
				eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
				   Constants.EVENT_CONFIG_COMMAND_APPROVE, user);
			}
			catch (Exception ex)
			{
				log.Error(ex);
			}

			return rs;
		}

		public bool CancelCommandDetail(int id, String user)
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
							cmd.CommandText = "Update_Command_By_CancelCommandDetail";
							cmd.CommandType = CommandType.StoredProcedure;
							SqlParameter param1 = new SqlParameter("ID", id);
							cmd.Parameters.Add(param1);
							cmd.ExecuteNonQuery();
							rs = true;
						}
						conn.Close();
					}
				}

				// Log event
				eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
				   Constants.EVENT_CONFIG_COMMAND_CANCEL_COMPARTMENT, user);
			}
			catch (Exception ex)
			{
				log.Error(ex);
			}

			return rs;
		}

        public bool UnApproveCommand(int commandId, String user) {
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
                            cmd.CommandText = "Update_Command_By_UnApproveCommand";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param1 = new SqlParameter("CommandID", commandId);
                            cmd.Parameters.Add(param1);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_COMMAND_UNAPPROVE, user);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool UpdateFlag(int Id, String user)
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
                            cmd.CommandText = "Update_Command_By_UpdateFlag";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param1 = new SqlParameter("ID", Id);
                            cmd.Parameters.Add(param1);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_COMMAND_UNAPPROVE, user);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public int getCertificateNumber()
        {
            var certificateNumber = 1;
            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Select_CertificateNumber";
                            cmd.CommandType = CommandType.StoredProcedure;   
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                certificateNumber = int.Parse(reader["CertificateNumber"].ToString()); 
                            }
                        }
                        conn.Close();
                    }
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_COMMAND_UNAPPROVE, "user");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return certificateNumber;
        }
        public bool CheckCommandID(int CommandID)
        {
            var CheckComand = false;
            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Select_Command_byID";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param1 = new SqlParameter("CommandID", CommandID);
                            SqlParameter param2 = new SqlParameter("TimeOrder", DateTime.Now);
                            cmd.Parameters.Add(param1);
                            cmd.Parameters.Add(param2);
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                CheckComand = true;
                            }
                        }
                        conn.Close();
                    }
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_COMMAND_UNAPPROVE, "user");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return CheckComand;
        }

        public bool UpdateCustomerCommandID(int commandId, string customerCode ,string user)
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
                            cmd.CommandText = "Update_Customer_By_CommandID";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param1 = new SqlParameter("CommandID", commandId);
                            SqlParameter param2 = new SqlParameter("CustomerCode", customerCode);
                            SqlParameter param3 = new SqlParameter("UpdateUser", user ?? "");
                            cmd.Parameters.Add(param1);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_COMMAND_UNAPPROVE, user);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }
    }
}