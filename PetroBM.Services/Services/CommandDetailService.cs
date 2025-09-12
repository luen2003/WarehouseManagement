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
using System.Activities.Validation;


namespace PetroBM.Services.Services
{
    public interface ICommandDetailService
    {
        IEnumerable<MCommandDetail> GetAllCommandDetail();
        IEnumerable<MCommandDetail> GetAllCommandDetailOrderByName();
        IEnumerable<MCommandDetail> GetAllCommandDetailOrderByCompartmentOrder();
        IEnumerable<MCommandDetail> GetAllCommandDetailByCardData(string cardDataCommandDetail);
        IEnumerable<MCommandDetail> GetAllCommandDetailByCardSerial(long cardSerial);
		IEnumerable<MCommandDetail> GetAllCommandDetailByWorkOrder(long workOrder);
		IEnumerable<MCommandDetail> GetAllCommandDetailByCommandID(int commandID);
        IEnumerable<MCommandDetail> GetAllCommandDetailByID(int commanddetailid);
        IEnumerable<MCommandDetail> GetAllCommandDetailByFlag(int flag);
        IEnumerable<MCommandDetail> GetAllCommandDetailByArmNo(byte armno);
        bool CreateCommandDetail(MCommandDetail commanddetail);
        bool UpdateCommandDetail(MCommandDetail commanddetail);
        //bool UpdateCommandDetailProduct(string productCode,string productName);
        bool UpdateListCommandDetail_By_CommandID_Flag(int commandId, byte flag,string updateUser);
        bool UpdateListCommandDetail_By_CommandID_Flag_2(int commandId, byte flag, string updateUser);
        bool UpdateListCommandDetail_By_CommandID_InvoiceId(int commandId, int invoiceId, string productCode, string listVolume, string updateUser);
        bool UpdateListCommandDetail_By_CommandID_InvoiceId_Flag(int commandId, int invoiceId, string productCode, string listVolume, string updateUser, byte flag);
        bool DeleteCommandDetail(int id);
        MCommandDetail FindCommandDetailById(int id);
        MCommandDetail FindFlagCommandDetailById(int id); 
        object GetAllCommandDetailOrderByName(string cardData, long cardSerial);

        bool CheckFlagBeforeCreateSeal(decimal? workOrder, byte? compartmentOrder);
        bool CalculateValue15(decimal? workOrder);

        bool CalculateV15();

        List<MCommandDetail> GetList_CommandDetail_By_Vehicle(byte? wareHouseCode, long? workOrder, string cardSerial, string cardData, DateTime? fromDate, DateTime? toDate, string vehicle, int? flag);

    }

    public class CommandDetailService : ICommandDetailService
    {
        ILog log = log4net.LogManager.GetLogger(typeof(CommandDetailService));
        private readonly ICommandDetailRepository commanddetailRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public CommandDetailService(ICommandDetailRepository commanddetailRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.commanddetailRepository = commanddetailRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateCommandDetail(MCommandDetail commanddetail)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    commanddetailRepository.Add(commanddetail);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                //eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                //    Constants.EVENT_CONFIG_COMMANDDETAIL_CREATE, commanddetail.InsertUser);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
            }
            catch (Exception e) 
            {
            }

            return rs;
        }

        public bool DeleteCommandDetail(int id)
        {
            MCommandDetail commanddetail = this.FindCommandDetailById(id);
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    commanddetail.DeleteFlg = Constants.FLAG_ON;
                    commanddetailRepository.Update(commanddetail);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_COMMANDDETAIL_DELETE, commanddetail.UpdateUser);
            }
            catch { }

            return rs;
        }

        public IEnumerable<MCommandDetail> GetAllCommandDetail()
        {
            return commanddetailRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(p => p.InsertDate);
        }

        public IEnumerable<MCommandDetail> GetAllCommandDetailOrderByName()
        {
            return commanddetailRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(p => p.ID);
        }

        public bool UpdateCommandDetail(MCommandDetail commanddetail)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    commanddetailRepository.Update(commanddetail);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                //eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                //   Constants.EVENT_CONFIG_COMMANDDETAIL_UPDATE, commanddetail.UpdateUser);
            }
            catch (Exception e) { }

            return rs;
        }
        //hiepsua 02/06/2020
        public bool UpdateListCommandDetail_By_CommandID_Flag_2(int commandId, byte flag, string updateUser)
        {
            var rs = false;
            try
            {
                var lstDetail = commanddetailRepository.GetAll().Where(x => x.CommandID == commandId && x.Flag ==Constants.Command_Flag_Seal &&  x.InvoiceDetailID!=null).ToList();
                foreach (var item in lstDetail)
                {
                    item.Flag = flag;
                    item.UpdateUser = updateUser;
                    using (TransactionScope ts = new TransactionScope())
                    {
                       
                        commanddetailRepository.Update(item);
                        unitOfWork.Commit();
                        ts.Complete();
                        rs = true;
                    }

                    // Log event
                    //eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    //   Constants.EVENT_CONFIG_COMMANDDETAIL_UPDATE, item.UpdateUser);

                }

            }
            catch (Exception e) { log.Info(e.Message); }

            return rs;
        }

        public bool UpdateListCommandDetail_By_CommandID_Flag( int commandId,byte flag , string updateUser)
        {
            var rs = false;
            try
            {
                var lstDetail = commanddetailRepository.GetAll().Where(x => x.CommandID == commandId).ToList();
                foreach (var item in lstDetail)
                {
                    item.Flag = flag;
                    item.UpdateUser = updateUser;
                    using (TransactionScope ts = new TransactionScope())
                    {
                        commanddetailRepository.Update(item);
                        unitOfWork.Commit();
                        ts.Complete();
                        rs = true;
                    }

                    // Log event
                    //eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    //   Constants.EVENT_CONFIG_COMMANDDETAIL_UPDATE, item.UpdateUser);

                }

            }
            catch (Exception e) { log.Info(e.Message); }

            return rs;
        }
        public bool UpdateListCommandDetail_By_CommandID_InvoiceId_Flag(int commandId, int invoiceId, string productCode, string listVolume, string updateUser, byte flag)
        {
            var rs = false;
            var arrCompartmentOrder = listVolume.ToCharArray();
            try
            {
                foreach (var compartment in arrCompartmentOrder)
                {
                    // && x.Flag == Constants.Command_Flag_Seal
                    var lstDetail = commanddetailRepository.GetAll().Where(x => x.CommandID == commandId && x.ProductCode == productCode && x.CompartmentOrder.ToString() == compartment.ToString()).ToList();
                    foreach (var item in lstDetail)
                    {
                        item.Flag = flag;
                        item.InvoiceDetailID = invoiceId;
                        item.UpdateUser = updateUser;
                        using (TransactionScope ts = new TransactionScope())
                        {
                            commanddetailRepository.Update(item);
                            unitOfWork.Commit();
                            ts.Complete();
                            rs = true;
                        }

                        // Log event
                        //eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                        //   Constants.EVENT_CONFIG_COMMANDDETAIL_UPDATE, item.UpdateUser);

                    }

                }


            }
            catch (Exception e) { log.Info(e.Message); }

            return rs;
        }
        public bool UpdateListCommandDetail_By_CommandID_InvoiceId(int commandId, int invoiceId, string productCode, string listVolume,string updateUser)
        {
            var rs = false;
            var arrCompartmentOrder = listVolume.ToCharArray();
            try
            {
                foreach (var compartment in arrCompartmentOrder)
                {
                    var lstDetail = commanddetailRepository.GetAll().Where(x => x.CommandID == commandId && x.ProductCode == productCode && x.CompartmentOrder.ToString() == compartment.ToString()).ToList();
                    foreach (var item in lstDetail)
                    {
                        item.InvoiceDetailID = invoiceId;
                        item.UpdateUser = updateUser;
                        using (TransactionScope ts = new TransactionScope())
                        {
                            commanddetailRepository.Update(item);
                            unitOfWork.Commit();
                            ts.Complete();
                            rs = true;
                        }

                        // Log event
                        //eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                        //   Constants.EVENT_CONFIG_COMMANDDETAIL_UPDATE, item.UpdateUser);

                    }
                }
               

            }
            catch (Exception e) { log.Info(e.Message); }

            return rs;
        }

        public IEnumerable<MCommandDetail> GetAllCommandDetailOrderByCompartmentOrder(){
            return commanddetailRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
               .OrderBy(p => p.CompartmentOrder);
        }

        public MCommandDetail FindCommandDetailById(int id)
        {
            return commanddetailRepository.GetById(id);
        }

        public IEnumerable<MCommandDetail> GetAllCommandDetailByCardData(string cardDataCommandDetail)
        {
            return commanddetailRepository.GetAll().Where(p => p.CardData == cardDataCommandDetail).OrderBy(p => p.ID);

        }

        public object GetAllCommandDetailOrderByName(string cardData, long cardSerial)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MCommandDetail> GetAllCommandDetailByCommandID(int commandID)
        {
            return commanddetailRepository.GetAll().Where(p => p.CommandID == commandID).OrderBy(p => p.ID);

        }

        public IEnumerable<MCommandDetail> GetAllCommandDetailByArmNo(byte armno)
        {
            return commanddetailRepository.GetAll().Where(p => p.ArmNo == armno)
               .OrderBy(p => p.ArmNo);
        }

        public IEnumerable<MCommandDetail> GetAllCommandDetailByFlag(int flag)
        {
            return commanddetailRepository.GetAll().Where(p => p.Flag == flag).OrderBy(p => p.ID);
        }

        public IEnumerable<MCommandDetail> GetAllCommandDetailByID(int commanddetailid)
        {
            return commanddetailRepository.GetAll().Where(p => p.ID == commanddetailid).OrderBy(p => p.ID);
        }

        public IEnumerable<MCommandDetail> GetAllCommandDetailByCardSerial(long cardSerial)
        {
            return commanddetailRepository.GetAll().Where(p => p.CardSerial == cardSerial).OrderBy(p => p.ID);
        }

		public IEnumerable<MCommandDetail> GetAllCommandDetailByWorkOrder(long workOrder)
		{
			return commanddetailRepository.GetAll().Where(p => p.WorkOrder == workOrder).OrderBy(p => p.ID);
		}

        public bool CheckFlagBeforeCreateSeal(decimal? workOrder, byte? compartmentOrder)
        {
            var rs = false;
            MCommandDetail commandDetail = commanddetailRepository.GetAll().Where(p => p.WorkOrder == workOrder && p.CompartmentOrder == compartmentOrder).OrderBy(p => p.ID).First();
            if(commandDetail.Flag == 8)
            {
                rs = true;
            }
            return rs;
        }

        public bool CalculateValue15(decimal? workOrder)
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
                            cmd.CommandText = "SPCaculateWcfVcf";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param1 = new SqlParameter("WorkOrder", workOrder);
                            cmd.Parameters.Add(param1);
                            cmd.ExecuteNonQuery();
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

        public bool CalculateV15()
        {
            log.Debug("Execute CalculateV15");
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
                            
                            cmd.CommandText = "SPCalculateVCFWCFInCompleteCommand";
                            cmd.CommandType = CommandType.StoredProcedure;
                            //SqlParameter param1 = new SqlParameter("WorkOrder", workOrder);
                            //cmd.Parameters.Add(param1);
                            cmd.ExecuteNonQuery();
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


        //public bool UpdateCommandDetailProduct(string productCode, string productName)
        //{
        //    var rs = false;
        //    try
        //    {
        //        var lstDetail = commanddetailRepository.GetAll().Where(x => x.ProductCode == productCode).ToList();
        //        foreach (var item in lstDetail)
        //        {
        //            item.ProductName = productName;
        //            using (TransactionScope ts = new TransactionScope())
        //            {
        //                commanddetailRepository.Update(item);
        //                unitOfWork.Commit();
        //                ts.Complete();
        //                rs = true;
        //            }

        //        }

        //    }
        //    catch (Exception e) { }

        //    return rs;
        //}

        public List<MCommandDetail> GetList_CommandDetail_By_Vehicle(byte? wareHouseCode, long? workOrder, string cardSerial, string cardData, DateTime? fromDate, DateTime? toDate, string vehicle, int? flag)
        {
            List<MCommandDetail> lst = new List<MCommandDetail>();

            try
            {

                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SelectList_CommandDetail_By_Vehicle";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param1 = new SqlParameter("WareHouseCode", wareHouseCode ?? 0);
                            SqlParameter param2 = new SqlParameter("WorkOrder", workOrder ?? 0);
                            SqlParameter param3 = new SqlParameter("CardSerial", cardSerial ?? "");
                            SqlParameter param4 = new SqlParameter("CardData", cardData ?? "");
                            SqlParameter param5 = new SqlParameter("FromDate", fromDate);
                            SqlParameter param6 = new SqlParameter("ToDate", toDate);
                            SqlParameter param7 = new SqlParameter("Vehicle", vehicle ?? "");
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
                                var it = new MCommandDetail();
                                it.CommandID = int.Parse(reader["CommandID"].ToString());
                                it.WorkOrder = decimal.Parse(reader["WorkOrder"].ToString());
                                //it.CustomerCode = reader["CustomerCode"].ToString();
                                it.CardData = reader["CardData"].ToString();
                                //it.CardSerial = long.Parse(reader["CardSerial"].ToString());
                                it.CardSerial = !string.IsNullOrEmpty(reader["CardSerial"].ToString())
                                ? long.Parse(reader["CardSerial"].ToString().Replace(",", ""))
                                : (long?)null;
                                it.TimeOrder = DateTime.Parse(reader["TimeOrder"].ToString());
                                it.V_Actual = int.Parse(reader["V_Actual"].ToString()); 
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

        public MCommandDetail FindFlagCommandDetailById(int id)
        {
            return commanddetailRepository.GetById(id);
        }
    }
}