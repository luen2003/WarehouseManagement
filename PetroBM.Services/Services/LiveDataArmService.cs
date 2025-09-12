using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Data;
using System.Data.SqlClient;
using PetroBM.Data;
using log4net;


namespace PetroBM.Services.Services
{
    public interface ILiveDataArmService
    {

        IEnumerable<MLiveDataArm> GetAllLiveDataArm();
        IEnumerable<MLiveDataArm> GetAllLiveDataArmOrderByName();
        IEnumerable<MLiveDataArm> GetAllLiveDataArmByArmNo(byte armno);
        IEnumerable<MLiveDataArm> GetAllLiveDataArmOrderByArmNo();
        IEnumerable<MLiveDataArm> GetAllLiveDataArmByWareHouse(byte warehousecode);
        IEnumerable<MLiveDataArm> GetAllLiveDataArmByWareHouseAndArmNo(byte warehousecode, byte armno);
        IEnumerable<MLiveDataArm> GetLiveDataArmByTime(byte? warehousecode, byte? armno, DateTime? startDate, DateTime? endDate);
        List<MLiveDataArm> GetLiveDataArmByArmNo(byte warehousecode,byte? armno);
        List<MLiveDataArm> GetLiveDataArm_By_GroupArm(byte warehousecode, int grouparm);

        List<MLiveDataArm> GetLiveDataArm_By_FromDate_ToDate(byte warehousecode, byte? armno, DateTime? fromDate, DateTime? toDate);        

        List<MLiveDataArm> GetLiveDataArm_HistoricalExport_By_FromDate_ToDate(byte warehousecode, byte armno, string productcode,string serialnumber, DateTime? fromDate, DateTime? toDate);

        bool CreateLiveDataArm(MLiveDataArm livedataarm);
        bool UpdateLiveDataArm(MLiveDataArm livedataarm);
        bool DeleteLiveDataArm(int id);

        MLiveDataArm FindLiveDataArmById(int id);

    }

    public class LiveDataArmService : ILiveDataArmService
    {
        private readonly ILiveDataArmRepository livedataarmRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(LiveDataArmService));

        public LiveDataArmService(ILiveDataArmRepository livedataarmRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.livedataarmRepository = livedataarmRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateLiveDataArm(MLiveDataArm livedataarm)
        {
            var rs = false;
            //try
            //{
            //    using (TransactionScope ts = new TransactionScope())
            //    {
            //        livedataarmRepository.Add(livedataarm);
            //        unitOfWork.Commit();
            //        ts.Complete();
            //        rs = true;
            //    }

            //    // Log event
            //    eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
            //        Constants.EVENT_CONFIG_PRODUCT, livedataarm.InsertUser);
            //}
            //catch (Exception e) { }

            return rs;
        }

        public bool DeleteLiveDataArm(int id)
        {
           // MLiveDataArm livedataarm = this.FindLiveDataArmById(id);
            var rs = false;
            //try
            //{
            //    using (TransactionScope ts = new TransactionScope())
            //    {
            //        livedataarm.DeleteFlg = Constants.FLAG_ON;
            //        livedataarmRepository.Update(livedataarm);
            //        unitOfWork.Commit();
            //        ts.Complete();

            //        rs = true;
            //    }

            //    // Log event
            //    eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
            //        Constants.EVENT_CONFIG_TANK_DELETE, livedataarm.UpdateUser);
            //}
            //catch { }

            return rs;
        }

        public IEnumerable<MLiveDataArm> GetAllLiveDataArm()
        {
            //return livedataarmRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
            //    .OrderBy(p => p.InsertDate);

            return livedataarmRepository.GetAll();
        }

        public IEnumerable<MLiveDataArm> GetAllLiveDataArmOrderByName()
        {
            //return livedataarmRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
            //    .OrderBy(p => p.TimeLog);
            return livedataarmRepository.GetAll().OrderBy(p => p.TimeLog);
        }

        public IEnumerable<MLiveDataArm> GetAllLiveDataArmOrderByArmNo()
        {
            return livedataarmRepository.GetAll().OrderBy(p => p.ArmNo); ;
        }

        public bool UpdateLiveDataArm(MLiveDataArm livedataarm)
        {
            var rs = false;
            //try
            //{
            //    using (TransactionScope ts = new TransactionScope())
            //    {
            //        livedataarmRepository.Update(livedataarm);
            //        unitOfWork.Commit();
            //        ts.Complete();
            //        rs = true;
            //    }

            //    // Log event
            //    eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
            //       Constants.EVENT_CONFIG_PRODUCT, livedataarm.UpdateUser);
            //}
            //catch (Exception e) { }

            return rs;
        }

        public MLiveDataArm FindLiveDataArmById(int id)
        {
            return livedataarmRepository.GetById(id);
        }

        public IEnumerable<MLiveDataArm> GetAllLiveDataArmByArmNo(byte armno)
        {
            return livedataarmRepository.GetAll().Where(p => p.ArmNo == armno);
        }

        public IEnumerable<MLiveDataArm> GetAllLiveDataArmByWareHouse(byte warehousecode)
        {
            return livedataarmRepository.GetAll().Where(p =>  p.WareHouseCode == warehousecode);
        }

        public IEnumerable<MLiveDataArm> GetAllLiveDataArmByWareHouseAndArmNo(byte warehousecode, byte armno)
        {
            return livedataarmRepository.GetAll().Where(p => p.WareHouseCode == warehousecode && p.ArmNo == armno);
        }

        public IEnumerable<MLiveDataArm> GetAllLiveDataArm(byte? warehousecode,byte? armno)
        {
            return livedataarmRepository.GetAll().Where(p =>  p.WareHouseCode == warehousecode && p.ArmNo == armno);
        }
        public IEnumerable<MLiveDataArm> GetLiveDataArmByTime(byte? warehousecode, byte? armno, DateTime? startdate, DateTime? enddate)
        {
            IEnumerable<MLiveDataArm> rs = null;

            if (warehousecode != null)
            {
                if (armno == null)
                {
                    if (startdate == null && enddate == null)
                    {
                        rs = GetAllLiveDataArmByWareHouse(warehousecode.Value);
                    }
                    else if (startdate == null)
                    {
                        rs = livedataarmRepository
                        .GetMany(tl => tl.TimeLog <= enddate)
                        .OrderBy(tl => tl.TimeLog)
                        .ThenByDescending(tl => tl.TimeLog);
                    }
                    else if (enddate == null)
                    {
                        rs = livedataarmRepository
                        .GetMany(tl => startdate <= tl.TimeLog)
                        .OrderBy(tl => tl.TimeLog)
                        .ThenByDescending(tl => tl.TimeLog);
                    }
                    else
                    {
                        rs = livedataarmRepository
                        .GetMany(tl => (startdate <= tl.TimeLog) && (tl.TimeLog <= enddate))
                        .OrderBy(tl => tl.TimeLog)
                        .ThenByDescending(tl => tl.TimeLog);
                    }
                }
                else
                {
                    if (startdate == null && enddate == null)
                    {
                        rs = GetAllLiveDataArm(warehousecode,armno);
                    }
                    else if (startdate == null)
                    {
                        rs = livedataarmRepository
                        .GetMany(tl => (armno == tl.ArmNo) && (tl.TimeLog <= enddate))
                        .OrderByDescending(tl => tl.TimeLog);
                    }
                    else if (enddate == null)
                    {
                        rs = livedataarmRepository
                        .GetMany(tl => (armno == tl.ArmNo) && (startdate <= tl.TimeLog))
                        .OrderByDescending(tl => tl.TimeLog);
                    }
                    else
                    {
                        rs = livedataarmRepository
                        .GetMany(tl => (armno == tl.ArmNo) && (startdate <= tl.TimeLog) && (tl.TimeLog <= enddate))
                        .OrderByDescending(tl => tl.TimeLog);
                    }
                }

            }

            return rs;
        }

        public List<MLiveDataArm> GetLiveDataArm_By_FromDate_ToDate(byte warehousecode, byte? armno, DateTime? fromDate, DateTime? toDate)
        {
            List<MLiveDataArm> lst = new List<MLiveDataArm>();
            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "GetLiveDataArm_By_WareHouseCode_ArmNo_FromDate_ToDate";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("WareHouseCode", warehousecode);
                            SqlParameter param2 = new SqlParameter("ArmNo", armno);
                            SqlParameter param3 = new SqlParameter("FromDate", fromDate);
                            SqlParameter param4 = new SqlParameter("ToDate", toDate);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.Parameters.Add(param4);
                            SqlDataReader reader = cmd.ExecuteReader();
                            var it = new MLiveDataArm();
                            while (reader.Read())
                            {

                                it.TimeLog = DateTime.Parse(reader["TimeLog"].ToString());
                                it.WareHouseCode= byte.Parse(reader["WareHouseCode"].ToString());
                                it.ArmNo= byte.Parse(reader["ArmNo"].ToString());
                                it.WorkOrder= decimal.Parse(reader["WorkOrder"].ToString());
                                it.CompartmentOrder= byte.Parse(reader["CompartmentOrder"].ToString());
                                it.ProductCode= reader["ProductCode"].ToString();
                                it.ProductName= reader["ProductName"].ToString();
                                it.VehicleNumber= reader["VehicleNumber"].ToString();
                                it.CardData= reader["CardData"].ToString();
                                it.CardSerial= long.Parse(reader["CardSerial"].ToString());
                                it.V_Preset= float.Parse(reader["V_Preset"].ToString());
                                it.V_Actual= float.Parse(reader["V_Actual"].ToString());
                                it.V_Actual_Base= float.Parse(reader["V_Actual_Base"].ToString());
                                it.V_Actual_E= float.Parse(reader["V_Actual_E"].ToString());
                                it.Flowrate= float.Parse(reader["Flowrate"].ToString());
                                it.Flowrate_Base= float.Parse(reader["Flowrate_Base"].ToString());
                                it.Flowrate_E= float.Parse(reader["Flowrate_E"].ToString());
                                it.AvgTemperature= float.Parse(reader["AvgTemperature"].ToString());
                                it.CurrentTemperature= float.Parse(reader["CurrentTemperature"].ToString());
                                it.MixingRatio= float.Parse(reader["MixingRatio"].ToString());
                                it.ActualRatio= float.Parse(reader["ActualRatio"].ToString());
                                it.Status= byte.Parse(reader["Status"].ToString());
                                it.ModeLog= byte.Parse(reader["ModeLog"].ToString());
                                it.ESD= byte.Parse(reader["ESD"].ToString());
                                //it.InsertDate= DateTime.Parse(reader["InsertDate"].ToString());
                                //it.InsertUser= reader["InsertUser"].ToString();
                                //it.UpdateDate= DateTime.Parse(reader["UpdateDate"].ToString());
                                //it.UpdateUser= reader["UpdateUser"].ToString();
                                //it.VersionNo= int.Parse(reader["VersionNo"].ToString());
                                it.ValveStatus= byte.Parse(reader["ValveStatus"].ToString());

                                lst.Add(it);
                            }
                        }
                        conn.Close();
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return lst;

        }



        public List<MLiveDataArm> GetLiveDataArm_HistoricalExport_By_FromDate_ToDate(byte warehousecode, byte armno, string productcode, string serialnumber, DateTime? fromDate, DateTime? toDate)
        {
            List<MLiveDataArm> lst = new List<MLiveDataArm>();           
            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "GetLiveDataArm_HistoricalExport_By_WareHousecode_ArmNo_ProductCode_SerialNumber_FromDate_ToDate";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("WareHouseCode", warehousecode);
                            SqlParameter param2 = new SqlParameter("TankId", armno);
                            SqlParameter param3 = new SqlParameter("FromDate", fromDate);
                            SqlParameter param4 = new SqlParameter("ToDate", toDate);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.Parameters.Add(param4);
                            SqlDataReader reader = cmd.ExecuteReader();
                            var it = new MLiveDataArm();
                            while (reader.Read())
                            {

                                it.TimeLog = DateTime.Parse(reader["TimeLog"].ToString());
                                it.WareHouseCode = byte.Parse(reader["WareHouseCode"].ToString());
                                it.ArmNo = byte.Parse(reader["ArmNo"].ToString());
                                it.WorkOrder = decimal.Parse(reader["WorkOrder"].ToString());
                                it.CompartmentOrder = byte.Parse(reader["CompartmentOrder"].ToString());
                                it.ProductCode = reader["ProductCode"].ToString();
                                it.ProductName = reader["ProductName"].ToString();
                                it.VehicleNumber = reader["VehicleNumber"].ToString();
                                it.CardData = reader["CardData"].ToString();
                                it.CardSerial = long.Parse(reader["CardSerial"].ToString());
                                it.V_Preset = float.Parse(reader["V_Preset"].ToString());
                                it.V_Actual = float.Parse(reader["V_Actual"].ToString());
                                it.V_Actual_Base = float.Parse(reader["V_Actual_Base"].ToString());
                                it.V_Actual_E = float.Parse(reader["V_Actual_E"].ToString());
                                it.Flowrate = float.Parse(reader["Flowrate"].ToString());
                                it.Flowrate_Base = float.Parse(reader["Flowrate_Base"].ToString());
                                it.Flowrate_E = float.Parse(reader["Flowrate_E"].ToString());
                                it.AvgTemperature = float.Parse(reader["AvgTemperature"].ToString());
                                it.CurrentTemperature = float.Parse(reader["CurrentTemperature"].ToString());
                                it.MixingRatio = float.Parse(reader["MixingRatio"].ToString());
                                it.ActualRatio = float.Parse(reader["ActualRatio"].ToString());
                                it.Status = byte.Parse(reader["Status"].ToString());
                                it.ModeLog = byte.Parse(reader["ModeLog"].ToString());
                                it.ESD = byte.Parse(reader["ESD"].ToString());
                                //it.InsertDate = DateTime.Parse(reader["InsertDate"].ToString());
                                //it.InsertUser = reader["InsertUser"].ToString();
                                //it.UpdateDate = DateTime.Parse(reader["UpdateDate"].ToString());
                                //it.UpdateUser = reader["UpdateUser"].ToString();
                                //it.VersionNo = int.Parse(reader["VersionNo"].ToString());
                                it.ValveStatus = byte.Parse(reader["ValveStatus"].ToString());

                                lst.Add(it);
                            }
                        }
                        conn.Close();
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return lst;

        }
        public List<MLiveDataArm> GetLiveDataArmByArmNo(byte warehousecode, byte? armno)
        {
            List<MLiveDataArm> lst = new List<MLiveDataArm>();
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "LiveDataArm_GetData";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("WareHouseCode", warehousecode);
                            SqlParameter param2 = new SqlParameter("ArmNo", armno);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            SqlDataReader reader = cmd.ExecuteReader();                            
                            while (reader.Read())
                            {
                                var it = new MLiveDataArm();
                                it.TimeLog = DateTime.Parse(reader["TimeLog"].ToString());
                                it.WareHouseCode = byte.Parse(reader["WareHouseCode"].ToString());
                                it.ArmNo = byte.Parse(reader["ArmNo"].ToString());
                                it.WorkOrder = decimal.Parse(reader["WorkOrder"].ToString());
                                it.CompartmentOrder = byte.Parse(reader["CompartmentOrder"].ToString());
                                it.ProductCode = reader["ProductCode"].ToString();
                                it.ProductName = reader["ProductName"].ToString();
                                it.VehicleNumber = reader["VehicleNumber"].ToString();
                                it.CardData = reader["CardData"].ToString();
                                it.CardSerial = long.Parse(reader["CardSerial"].ToString());
                                it.V_Preset = float.Parse(string.IsNullOrEmpty(reader["V_Preset"].ToString()) ? "0" : reader["V_Preset"].ToString());
                                it.V_Actual = float.Parse(string.IsNullOrEmpty(reader["V_Actual"].ToString()) ? "0" : reader["V_Actual"].ToString());
                                it.V_Actual_Base = float.Parse(string.IsNullOrEmpty(reader["V_Actual_Base"].ToString()) ? "0" : reader["V_Actual_Base"].ToString());
                                it.V_Actual_E = float.Parse(string.IsNullOrEmpty(reader["V_Actual_E"].ToString()) ? "0" : reader["V_Actual_E"].ToString());
                                it.Flowrate = float.Parse(string.IsNullOrEmpty(reader["Flowrate"].ToString()) ? "0" : reader["Flowrate"].ToString());
                                it.Flowrate_Base = float.Parse(string.IsNullOrEmpty(reader["Flowrate_Base"].ToString()) ? "0" : reader["Flowrate_Base"].ToString());
                                it.Flowrate_E = float.Parse(string.IsNullOrEmpty(reader["Flowrate_E"].ToString()) ? "0" : reader["Flowrate_E"].ToString());
                                it.AvgTemperature = float.Parse(string.IsNullOrEmpty(reader["AvgTemperature"].ToString()) ? "0" : reader["AvgTemperature"].ToString());
                                it.CurrentTemperature = float.Parse(string.IsNullOrEmpty(reader["CurrentTemperature"].ToString()) ? "0" : reader["CurrentTemperature"].ToString());
                                it.MixingRatio = float.Parse(string.IsNullOrEmpty(reader["MixingRatio"].ToString()) ? "0" : reader["MixingRatio"].ToString());
                                it.ActualRatio = float.Parse(string.IsNullOrEmpty(reader["ActualRatio"].ToString()) ? "0" : reader["ActualRatio"].ToString());
                                it.Status = byte.Parse(string.IsNullOrEmpty(reader["Status"].ToString()) ? "0" : reader["Status"].ToString());
                                it.ModeLog = byte.Parse(string.IsNullOrEmpty(reader["ModeLog"].ToString()) ? "0" : reader["ModeLog"].ToString());
                                it.ESD = byte.Parse(string.IsNullOrEmpty(reader["ESD"].ToString()) ? "0" : reader["ESD"].ToString());
                                it.ValveStatus = byte.Parse(string.IsNullOrEmpty(reader["ValveStatus"].ToString()) ? "0" : reader["ValveStatus"].ToString());
                                it.CommandType = byte.Parse(string.IsNullOrEmpty(reader["CommandType"].ToString()) ? "2" : reader["CommandType"].ToString());
                                it.Abbreviations = reader["Abbreviations"].ToString();
                                lst.Add(it);
                            }
                        }
                        conn.Close();
                    }
                }

            }
            return lst;
        }

        public List<MLiveDataArm> GetLiveDataArm_By_GroupArm(byte warehousecode, int grouparm)
        {
            List<MLiveDataArm> lst = new List<MLiveDataArm>();
            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "GetLiveDataArm_By_GroupArm";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("WareHouseCode", warehousecode);
                            SqlParameter param2 = new SqlParameter("GroupArm", grouparm);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            SqlDataReader reader = cmd.ExecuteReader();
                            
                            while (reader.Read())
                            {
                                
                                var it = new MLiveDataArm();
                                it.TimeLog = DateTime.Parse(reader["TimeLog"].ToString());                                
                                it.WareHouseCode = byte.Parse(reader["WareHouseCode"].ToString());
                                it.ArmNo = byte.Parse(reader["ArmNo"].ToString());
                                it.WorkOrder = decimal.Parse(reader["WorkOrder"].ToString());
                                it.CompartmentOrder = byte.Parse(reader["CompartmentOrder"].ToString());                                
                                it.ProductCode = reader["ProductCode"].ToString();
                                it.ProductName = reader["ProductName"].ToString();
                                it.VehicleNumber = reader["VehicleNumber"].ToString();
                                it.CardData = reader["CardData"].ToString();
                                it.CardSerial = long.Parse(reader["CardSerial"].ToString());
                                it.V_Preset = float.Parse(string.IsNullOrEmpty(reader["V_Preset"].ToString())?"0": reader["V_Preset"].ToString());
                                it.V_Actual = float.Parse(string.IsNullOrEmpty(reader["V_Actual"].ToString()) ? "0" : reader["V_Actual"].ToString());
                                it.V_Actual_Base = float.Parse(string.IsNullOrEmpty(reader["V_Actual_Base"].ToString()) ? "0" : reader["V_Actual_Base"].ToString());
                                it.V_Actual_E = float.Parse(string.IsNullOrEmpty(reader["V_Actual_E"].ToString()) ? "0" : reader["V_Actual_E"].ToString());
                                it.Flowrate = float.Parse(string.IsNullOrEmpty(reader["Flowrate"].ToString()) ? "0" : reader["Flowrate"].ToString());
                                it.Flowrate_Base = float.Parse(string.IsNullOrEmpty(reader["Flowrate_Base"].ToString()) ? "0" : reader["Flowrate_Base"].ToString());
                                it.Flowrate_E = float.Parse(string.IsNullOrEmpty(reader["Flowrate_E"].ToString()) ? "0" : reader["Flowrate_E"].ToString());
                                it.AvgTemperature = float.Parse(string.IsNullOrEmpty(reader["AvgTemperature"].ToString()) ? "0" : reader["AvgTemperature"].ToString());
                                it.CurrentTemperature = float.Parse(string.IsNullOrEmpty(reader["CurrentTemperature"].ToString()) ? "0" : reader["CurrentTemperature"].ToString());
                                it.MixingRatio = float.Parse(string.IsNullOrEmpty(reader["MixingRatio"].ToString()) ? "0" : reader["MixingRatio"].ToString());
                                it.ActualRatio = float.Parse(string.IsNullOrEmpty(reader["ActualRatio"].ToString()) ? "0" : reader["ActualRatio"].ToString());
                                it.Status = byte.Parse(string.IsNullOrEmpty(reader["Status"].ToString()) ? "0" : reader["Status"].ToString());
                                it.ModeLog = byte.Parse(string.IsNullOrEmpty(reader["ModeLog"].ToString()) ? "0" : reader["ModeLog"].ToString());
                                it.ESD = byte.Parse(string.IsNullOrEmpty(reader["ESD"].ToString()) ? "0" : reader["ESD"].ToString());
                                it.ValveStatus = byte.Parse(string.IsNullOrEmpty(reader["ValveStatus"].ToString()) ? "0" : reader["ValveStatus"].ToString());
                                it.CommandType = byte.Parse(string.IsNullOrEmpty(reader["CommandType"].ToString()) ? "0" : reader["CommandType"].ToString());
                                it.Abbreviations = reader["Abbreviations"].ToString();
                                lst.Add(it);
                            }
                        }
                        conn.Close();
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return lst;

        }

    }
}