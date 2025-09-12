using log4net;
using PetroBM.Common.Util;
using PetroBM.Data;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Web;

namespace PetroBM.Services.Services
{
    public interface IDispatchService
    {
        IEnumerable<MDispatch> GetAllDispatch(); 
        IEnumerable<MDispatch> GetAllDispatchByID(int dispatchID);

        int getCertificateNumber();
        decimal? GetWorkOrderMax();

        bool CreateDispatch(MDispatch dispatch);

        int GetNewId();

        MDispatch FindDispatchById(int id);

        List<MDispatch> GetList_Dispatch(byte? wareHouseCode, string certificateNumber, string DstPickup1, string DstPickup2, string dstReceive, DateTime? fromDate, DateTime? toDate);

        List<MDispatch> GetList_Dispatch_byID(string dispatchID);

        bool DeleteDispatch(int dispatch);

        bool UpdateDispatch(int dispatchId, string timeStart, string timeStop, string vehicle, string product, string driverName1, string driverName2, string dstPickup1, string dstPickup2, string department, string note, string remark, string dstReceive, string user);

    }

    public class DispatchService : IDispatchService
    {

        ILog log = log4net.LogManager.GetLogger(typeof(DispatchService));
        private readonly IDispatchRepository dispatchRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public DispatchService(IDispatchRepository IdispatchRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.dispatchRepository = IdispatchRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public IEnumerable<MDispatch> GetAllDispatch()
        {
            return dispatchRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(p => p.InsertDate);
        }

        public IEnumerable<MDispatch> GetAllDispatchByID(int dispatchID)
        {
            return dispatchRepository.GetAll().Where(p => p.DispatchID == dispatchID).OrderBy(p => p.DispatchID);
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
                            cmd.CommandText = "Select_MDispatch";
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

        public decimal? GetWorkOrderMax()
        {
            var certificateNumberMax = dispatchRepository.GetAll().Max(x => x.DispatchID);

            return dispatchRepository.GetById(certificateNumberMax).CertificateNumber;
        }

        public bool CreateDispatch(MDispatch dispatch)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    dispatchRepository.Add(dispatch);
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

        public int GetNewId()
        {
            return dispatchRepository.GetAll().Max(x => x.DispatchID);
        }

        public MDispatch FindDispatchById(int id)
        {
            return dispatchRepository.GetById(id);
        }

        public List<MDispatch> GetList_Dispatch(byte? wareHouseCode, string certificateNumber, string dstPickup1, string dstPickup2, string dstReceive, DateTime? fromDate, DateTime? toDate)
        {
            List<MDispatch> lst = new List<MDispatch>();

            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SelectList_Dispatch";

                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param1 = new SqlParameter("WareHouseCode", wareHouseCode ?? 0);
                            SqlParameter param2 = new SqlParameter("CertificateNumber", certificateNumber ?? "");
                            SqlParameter param3 = new SqlParameter("DstPickup1", dstPickup1 ?? "");
                            SqlParameter param4 = new SqlParameter("DstPickup2", dstPickup2 ?? "");
                            SqlParameter param5 = new SqlParameter("DstReceive", dstReceive ?? "");
                            SqlParameter param6 = new SqlParameter("FromDate", fromDate);
                            SqlParameter param7 = new SqlParameter("ToDate", toDate); 

                            cmd.Parameters.Add(param1);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.Parameters.Add(param4);
                            cmd.Parameters.Add(param5);
                            cmd.Parameters.Add(param6);
                            cmd.Parameters.Add(param7); 
                            SqlDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                var it = new MDispatch();
                                it.DispatchID = int.Parse(reader["DispatchID"].ToString());
                                it.CertificateNumber = int.Parse(reader["CertificateNumber"].ToString());
                                it.TimeOrder = DateTime.Parse(reader["TimeOrder"].ToString());
                                it.TimeStart = DateTime.Parse(reader["TimeStart"].ToString());
                                it.TimeStop = DateTime.Parse(reader["TimeStop"].ToString());
                                it.VehicleNumber = reader["VehicleNumber"].ToString();
                                it.DriverName1 = reader["DriverName1"].ToString();
                                it.DriverName2 = reader["DriverName2"].ToString();
                                it.ProductCode = reader["ProductCode"].ToString();
                                it.Department = reader["Department"].ToString();
                                it.DstPickup1 = reader["DstPickup1"].ToString();
                                it.DstPickup2 = reader["DstPickup2"].ToString();
                                it.Note1 = reader["Note1"].ToString();
                                it.DstReceive = reader["DstReceive"].ToString();
                                it.Remark = reader["Remark"].ToString(); 
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

        public List<MDispatch> GetList_Dispatch_byID(string dispatchID)
        {
            List<MDispatch> lst = new List<MDispatch>();

            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SelectList_Dispatch_ByID";

                            cmd.CommandType = CommandType.StoredProcedure; 
                            SqlParameter param1 = new SqlParameter("DispatchID", dispatchID ?? "");

                            cmd.Parameters.Add(param1); 
                            SqlDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                var it = new MDispatch();
                                it.DispatchID = int.Parse(reader["DispatchID"].ToString());
                                it.CertificateNumber = int.Parse(reader["CertificateNumber"].ToString());
                                it.TimeOrder = DateTime.Parse(reader["TimeOrder"].ToString());
                                it.TimeStart = DateTime.Parse(reader["TimeStart"].ToString());
                                it.TimeStop = DateTime.Parse(reader["TimeStop"].ToString());
                                it.VehicleNumber = reader["VehicleNumber"].ToString();
                                it.DriverName1 = reader["DriverName1"].ToString();
                                it.DriverName2 = reader["DriverName2"].ToString();
                                it.ProductCode = reader["ProductCode"].ToString();
                                it.Department = reader["Department"].ToString();
                                it.DstPickup1 = reader["DstPickup1"].ToString();
                                it.DstPickup2 = reader["DstPickup2"].ToString();
                                it.Note1 = reader["Note1"].ToString();
                                it.DstReceive = reader["DstReceive"].ToString();
                                it.Remark = reader["Remark"].ToString();
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

        public bool DeleteDispatch(int dispatchID)
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
                            cmd.CommandText = "Update_DeleteFlg_Dispatch_By_ID";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param1 = new SqlParameter("DispatchID", dispatchID);
                            cmd.Parameters.Add(param1);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_COMMAND_MOVE, "user");
            }
            catch (Exception ex)
            {

            }

            return rs;
        }

        public bool UpdateDispatch(int dispatchId, string timeStart, string timeStop, string vehicle, string product, string driverName1, string driverName2, string dstPickup1, string dstPickup2, string department, string note, string remark, string dstReceive, string user)
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
                            cmd.CommandText = "Update_Dispatch_ByID";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param1 = new SqlParameter("DispatchID", dispatchId);
                            SqlParameter param2 = new SqlParameter("TimeStart", DateTime.ParseExact(timeStart, Constants.DATE_FORMAT, CultureInfo.InvariantCulture));
                            SqlParameter param3 = new SqlParameter("TimeStop", DateTime.ParseExact(timeStop, Constants.DATE_FORMAT, CultureInfo.InvariantCulture));
                            SqlParameter param4 = new SqlParameter("VehicleNumber", vehicle ?? "");
                            SqlParameter param5 = new SqlParameter("ProductCode", product ?? "");
                            SqlParameter param6 = new SqlParameter("DriverName1", driverName1 ?? "");
                            SqlParameter param7 = new SqlParameter("DriverName2", driverName2 ?? "");
                            SqlParameter param8 = new SqlParameter("DstPickup1", dstPickup1 ?? "");
                            SqlParameter param9 = new SqlParameter("DstPickup2", dstPickup2 ?? "");
                            SqlParameter param10 = new SqlParameter("Department", department ?? "");
                            SqlParameter param11 = new SqlParameter("Note1", note ?? "");
                            SqlParameter param12 = new SqlParameter("Remark", remark ?? "");
                            SqlParameter param13 = new SqlParameter("DstReceive", dstReceive ?? "");
                            SqlParameter param14 = new SqlParameter("User", user);
                            cmd.Parameters.Add(param1);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.Parameters.Add(param4);
                            cmd.Parameters.Add(param5);
                            cmd.Parameters.Add(param6);
                            cmd.Parameters.Add(param7);
                            cmd.Parameters.Add(param8);
                            cmd.Parameters.Add(param9);
                            cmd.Parameters.Add(param10);
                            cmd.Parameters.Add(param11);
                            cmd.Parameters.Add(param12);
                            cmd.Parameters.Add(param13);
                            cmd.Parameters.Add(param14);

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
                Console.WriteLine(ex.ToString());
                log.Error(ex.ToString());
            }

            return rs;
        }
    }
}