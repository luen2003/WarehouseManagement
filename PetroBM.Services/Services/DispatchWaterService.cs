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
using System.Web.Util;

namespace PetroBM.Services.Services
{
    public interface IDispatchWaterService
    {
        IEnumerable<MDispatchWater> GetAllDispatch();
        IEnumerable<MDispatchWater> GetAllDispatchByID(int dispatchID);

        int getCertificateNumber();
        decimal? GetWorkOrderMax();

        bool CreateDispatch(MDispatchWater dispatch);

        int GetNewId();

        MDispatchWater FindDispatchWaterById(int id);

        List<MDispatchWater> GetList_Dispatch(byte? wareHouseCode, string certificateNumber, string DstPickup1, string DstPickup2, string dstReceive, DateTime? fromDate, DateTime? toDate, string From, string To, string Paragraph1, string Paragraph2, string Paragraph3, string Paragraph4);

        List<MDispatchWater> GetList_Dispatch_byID(string dispatchID);

        bool DeleteDispatch(int dispatch);

        bool UpdateDispatch(int dispatchId, string timeStart, string timeStop, string vehicle, string product, string driverName1, string driverName2, string dstPickup1, string dstPickup2, string department, string note, string remark, string dstReceive, string From, string To, string Paragraph1, string Paragraph2, string Paragraph3, string Paragraph4, string user);

        string GetTransactionId(int dispatchId);

        void UpdateTransactionId(int dispatchId, string transactionId);

        bool UpdateProcessStatusById(int dispatchId);

    }

    public class DispatchWaterService : IDispatchWaterService
    {

        ILog log = log4net.LogManager.GetLogger(typeof(DispatchService));
        private readonly IDispatchWaterRepository dispatchWaterRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public DispatchWaterService(IDispatchWaterRepository IdispatchRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.dispatchWaterRepository = IdispatchRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public IEnumerable<MDispatchWater> GetAllDispatch()
        {
            return dispatchWaterRepository.GetAll()
                .OrderByDescending(p => p.InsertDate);
        }

        public IEnumerable<MDispatchWater> GetAllDispatchByID(int dispatchID)
        {
            return dispatchWaterRepository.GetAll().Where(p => p.DispatchID == dispatchID).OrderBy(p => p.DispatchID);
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
                            cmd.CommandText = "Select_MDispatchWater";
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
            var certificateNumberMax = dispatchWaterRepository.GetAll().Max(x => x.DispatchID);

            return dispatchWaterRepository.GetById(certificateNumberMax).CertificateNumber;
        }

        public bool CreateDispatch(MDispatchWater dispatch)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    dispatchWaterRepository.Add(dispatch);
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
            return dispatchWaterRepository.GetAll().Max(x => x.DispatchID);

        }

        public MDispatchWater FindDispatchWaterById(int id)
        {
            return dispatchWaterRepository.GetById(id);

        }


        public List<MDispatchWater> GetList_Dispatch(byte? wareHouseCode, string certificateNumber, string dstPickup1, string dstPickup2, string dstReceive, DateTime? fromDate, DateTime? toDate, string From, string To, string Paragraph1, string Paragraph2, string Paragraph3, string Paragraph4)
        {
            List<MDispatchWater> lst = new List<MDispatchWater>();

            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SelectList_DispatchWater";

                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param1 = new SqlParameter("WareHouseCode", wareHouseCode ?? 0);
                            SqlParameter param2 = new SqlParameter("CertificateNumber", certificateNumber ?? "");
                            SqlParameter param3 = new SqlParameter("DstPickup1", dstPickup1 ?? "");
                            SqlParameter param4 = new SqlParameter("DstPickup2", dstPickup2 ?? "");
                            SqlParameter param5 = new SqlParameter("DstReceive", dstReceive ?? "");
                            SqlParameter param6 = new SqlParameter("FromDate", fromDate);
                            SqlParameter param7 = new SqlParameter("ToDate", toDate);
                            SqlParameter param8 = new SqlParameter("From", From ?? "");
                            SqlParameter param9 = new SqlParameter("To", To ?? "");
                            SqlParameter param10 = new SqlParameter("Paragraph1", Paragraph1 ?? "");
                            SqlParameter param11 = new SqlParameter("Paragraph2", Paragraph2 ?? "");
                            SqlParameter param12 = new SqlParameter("Paragraph3", Paragraph3 ?? "");
                            SqlParameter param13 = new SqlParameter("Paragraph4", Paragraph4 ?? "");

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
                            SqlDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                var it = new MDispatchWater();
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
                                it.From = reader["From"].ToString();
                                it.To = reader["To"].ToString();
                                it.Paragraph1 = reader["Paragraph1"].ToString();
                                it.Paragraph2 = reader["Paragraph2"].ToString();
                                it.Paragraph3 = reader["Paragraph3"].ToString();
                                it.Paragraph4 = reader["Paragraph4"].ToString();
                                it.Remark = reader["Remark"].ToString();
                                it.ProcessStatus = int.Parse(reader["ProcessStatus"].ToString());
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

        public List<MDispatchWater> GetList_Dispatch_byID(string dispatchID)
        {
            List<MDispatchWater> lst = new List<MDispatchWater>();

            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SelectList_DispatchWater_ByID";
                            cmd.CommandType = CommandType.StoredProcedure;

                            SqlParameter param1 = new SqlParameter("DispatchID", dispatchID ?? "");
                            cmd.Parameters.Add(param1);

                            SqlDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                var it = new MDispatchWater();
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
                                it.From = reader["From"].ToString();
                                it.To = reader["To"].ToString();
                                it.Paragraph1 = reader["Paragraph1"].ToString();
                                it.Paragraph2 = reader["Paragraph2"].ToString();
                                it.Paragraph3 = reader["Paragraph3"].ToString();
                                it.Paragraph4 = reader["Paragraph4"].ToString();
                                it.Remark = reader["Remark"].ToString();
                                it.ProcessStatus = int.Parse(reader["ProcessStatus"].ToString());
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
                // Đừng return null, cứ return danh sách rỗng
            }

            return lst;
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
                            cmd.CommandText = "Update_DeleteFlg_DispatchWater_By_ID";
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

        public string GetTransactionId(int dispatchId)
        {
            string transactionId = null;

            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Select_TransactionId_DispatchWater_ByID";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add(new SqlParameter("DispatchID", dispatchId));

                            var reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                transactionId = reader["TransactionId"]?.ToString();
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi lấy TransactionId cho DispatchID {dispatchId}: {ex}");
                throw;
            }

            return transactionId;
        }


        public bool UpdateDispatch(int dispatchId, string timeStart, string timeStop, string vehicle, string product, string driverName1, string driverName2, string dstPickup1, string dstPickup2, string department, string note, string remark, string dstReceive, string From, string To, string Paragraph1, string Paragraph2, string Paragraph3, string Paragraph4, string user)
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
                            cmd.CommandText = "Update_DispatchWater_ByID";
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
                            SqlParameter param14 = new SqlParameter("From", From ?? "");
                            SqlParameter param15 = new SqlParameter("To", To ?? "");
                            SqlParameter param16 = new SqlParameter("Paragraph1", Paragraph1 ?? "");
                            SqlParameter param17 = new SqlParameter("Paragraph2", Paragraph2 ?? "");
                            SqlParameter param18 = new SqlParameter("Paragraph3", Paragraph3 ?? "");
                            SqlParameter param19 = new SqlParameter("Paragraph4", Paragraph4 ?? "");
                            SqlParameter param20 = new SqlParameter("User", user);
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
                            cmd.Parameters.Add(param15);
                            cmd.Parameters.Add(param16);
                            cmd.Parameters.Add(param17);
                            cmd.Parameters.Add(param18);
                            cmd.Parameters.Add(param19);
                            cmd.Parameters.Add(param20);
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


        public void UpdateTransactionId(int dispatchId, string transactionId)
        {
            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Update_TransactionId_DispatchWater_ByID";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add(new SqlParameter("DispatchID", dispatchId));
                            cmd.Parameters.Add(new SqlParameter("TransactionId", transactionId ?? ""));

                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                }

                // Log event (nếu muốn)
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_COMMAND_UPDATE, $"Cập nhật TransactionId cho DispatchID {dispatchId}");
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi cập nhật TransactionId cho DispatchID {dispatchId}: {ex}");
                throw; // có thể throw lên để controller xử lý
            }
        }

        public bool UpdateProcessStatusById(int dispatchId)
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
                            cmd.CommandText = "Update_ProcessStatus_DispatchWater_ByID";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add(new SqlParameter("DispatchID", dispatchId));

                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }

                // Log event (nếu muốn)
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_COMMAND_UPDATE, $"Cập nhật ProcessStatus cho DispatchID {dispatchId}");
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi cập nhật ProcessStatus cho DispatchID {dispatchId}: {ex}");
                throw; // có thể throw lên để controller xử lý
            }
            return rs;
        }
    }
}