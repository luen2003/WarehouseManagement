using PetroBM.Common.Util;
using PetroBM.Data;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using log4net;


namespace PetroBM.Services.Services
{
    public interface ITankService
    {
        IEnumerable<MTank> GetAllTank();
        IEnumerable<MTank> GetAllTankOrderByName();
        IEnumerable<MTank> GetAllTankOrderById();
        IEnumerable<MTank> GetTankIdByTankName(string tankname);
        bool CreateTank(MTank tank);
        bool UpdateTank(MTank tank);
        bool UpdateVolumeMax(int id);
        bool DeleteTank(int id, byte wareHouseCode);
        MTank GetTankByID(int id);
        MTank FindTankById(int id);
        MTank FindTankById(int tankid, byte wareHouseCode);
        IEnumerable<MTank> GetAllTankByProduct(int productId);
        IEnumerable<MTank> GetAllTankByWareHouseCode(byte warehousecode);

        IEnumerable<MTankDensity> GetAllTankDensity(int tankId);

        IEnumerable<MTankDensity> GetAllTankDensity(int tankId, byte warehousecode);
        bool CreateTankDensity(MTankDensity tankDensity);

        IEnumerable<MTankLive> GetNewestTankLive();

        IEnumerable<MLiveDataArm> GetNewestLiveDataArm();

        MTankLive GetNewestTankLive(int tankId);

        MTankLive GetNewestTankLive(int tankId, byte wareHouseCode);
        bool CheckDataServerRunning();

        IEnumerable<MTankManual> GetAllTankManual();
        IEnumerable<MTankManual> GetAllTankManual(int tankId);
        IEnumerable<MTankManual> GetTankManualByTime(int? tankId, DateTime? startDate, DateTime? endDate);
        MTankManual GetTankManual(int id);
        bool CreateTankManual(MTankManual tankManual);
        bool UpdateTankManual(MTankManual tankManual);
        bool DeleteTankManual(MTankManual tankManual);

        MTankLog GetTankLogByTime(int tankId, DateTime date);

        MTankLog GetTankLogByTime(byte wareHouseCode, int tankId, DateTime date);

        IEnumerable<MTankLog> GetAllDataTankLog(byte wareHouse, int productId, DateTime? startDate, DateTime? endDate);
        IEnumerable<MTankLog> GetAllTankLog(byte wareHouseCode, int tankId);
        IEnumerable<MTankLog> GetTankLogByTime(int? tankId, DateTime? startDate, DateTime? endDate);
        IEnumerable<MTankLog> GetTankLogByTime(byte? wareHouseCode, int? tankId, DateTime? startDate, DateTime? endDate);

        double GetTankTurnOverNumber(int id, DateTime startDate, DateTime endDate);
        double GetTankExport(int id, DateTime startDate, DateTime endDate);
        double GetUsagePerformanceOfTank(int id, DateTime startDate, DateTime endDate);
        double GetAvgProductVolumeInTank(int id, DateTime startDate, DateTime endDate);
        double GetRepositoryTurnOverNumber(DateTime startDate, DateTime endDate);
        double GetRepositoryExport(DateTime startDate, DateTime endDate);
        double GetUsagePerformanceOfRepository(DateTime startDate, DateTime endDate);
        double GetAvgProductVolumeInRepository(DateTime startDate, DateTime endDate);
        double GetRepositoryVolume();
        double GetProductOut(byte wareHouseCode,int productId, DateTime startDate, DateTime endDate);

        IEnumerable<MBarem> GetBarem(int tankId, byte wareHouseCode);
        MBarem GetBaremByHigh(int tankId, float high);
        MBarem GetBaremByHigh(byte wareHouseCode, int tankId, float high);

        bool CreateBarem(MBarem barem);
        bool UpdateBarem(MBarem barem);
        bool DeleteBarem(int tankId, IEnumerable<float> highs, string user);


        //bool ImportBarem(int tankId, HttpPostedFileBase file, string user);

        bool ImportBarem(byte wareHouseCode, int tankId, HttpPostedFileBase file, string user);

        int GetMaxTankId_By_WareHouseCode(byte wareHouseCode);

        double SearchVolume(int id, float high);
        float SearchVCF(float d15, float t);

        List<MTankLive> GetTankLive_Greater_Time(byte warehousecode, int tankid, DateTime greaterTime);
    }

    public class TankService : ITankService
    {
        ILog log = log4net.LogManager.GetLogger(typeof(TankService)); 

        private readonly ITankRepository tankRepository;
        private readonly ITankDensityRepository tankDensityRepository;
        private readonly ITankManualRepository tankManualRepository;
        private readonly ITankLiveRepository tankLiveRepository;
        private readonly ILiveDataArmRepository liveDataArmRepository;

        private readonly ITankLogRepository tankLogRepository;
        private readonly IBaremRepository baremRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;


        public TankService(ITankRepository tankRepository, ITankDensityRepository tankDensityRepository,
            ITankManualRepository tankManualRepository, ITankLiveRepository tankLiveRepository, ILiveDataArmRepository liveDataArmRepository,
        ITankLogRepository tankLogRepository, IBaremRepository baremRepository, IUnitOfWork unitOfWork,
            IEventService eventService)
        {
            this.tankRepository = tankRepository;
            this.tankDensityRepository = tankDensityRepository;
            this.tankManualRepository = tankManualRepository;
            this.tankLiveRepository = tankLiveRepository;
            this.tankLogRepository = tankLogRepository;
            this.baremRepository = baremRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
            this.liveDataArmRepository = liveDataArmRepository;
        }

        public bool CreateTank(MTank tank)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    tank.UpdateUser = tank.InsertUser;
                    tank.TankId = GetMaxTankId_By_WareHouseCode(tank.WareHouseCode) + 1;
                    tankRepository.Add(tank);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_TANK_CREATE, tank.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool DeleteTank(int id)
        {
            MTank tank = this.FindTankById(id);
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    tank.DeleteFlg = Constants.FLAG_ON;
                    tankRepository.Update(tank);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_TANK_DELETE, tank.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }
        public bool DeleteTank(int id, byte wareHouseCode)
        {
            MTank tank = this.FindTankById(id, wareHouseCode);
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    tank.DeleteFlg = Constants.FLAG_ON;
                    tankRepository.Update(tank);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_TANK_DELETE, tank.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }
        public IEnumerable<MTank> GetAllTank()
        {
            return tankRepository.GetAll().Where(t => (t.TankId != Constants.TEC_CC_ID) && (t.DeleteFlg == Constants.FLAG_OFF))
                .OrderByDescending(tm => tm.InsertDate);
        }

        public IEnumerable<MTank> GetAllTankOrderByName()
        {
            return tankRepository.GetAll().Where(t => (t.TankId != Constants.TEC_CC_ID) && (t.DeleteFlg == Constants.FLAG_OFF))
                .OrderBy(tm => tm.TankName);
        }

        public IEnumerable<MTank> GetAllTankOrderById()
        {
            return tankRepository.GetAll().Where(t => (t.TankId != Constants.TEC_CC_ID) && (t.DeleteFlg == Constants.FLAG_OFF))
                .OrderBy(tm => tm.TankId);
        }

        public bool UpdateTank(MTank tank)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    tankRepository.Update(tank);
                    unitOfWork.Commit();
                    // UpdateVolumeMax(tank.TankId); //Chua tim thay ham nay
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_TANK_UPDATE, tank.UpdateUser);

                ResetHighMax(tank.TankId, tank.WareHouseCode);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool UpdateVolumeMax(int id)
        {
            bool rs = false;
            double highMax = 0;
            var tank = FindTankById(id);
            if (tank.HighMax != null)
            {
                highMax = Math.Round((double)tank.HighMax);
            }
            try
            {
                using (var context = new PetroBMContext())
                {
                    var high = new SqlParameter("@High", highMax);
                    var tankId = new SqlParameter("@TankId", id);
                    var volume = new SqlParameter("@Volume", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    context.Database.ExecuteSqlCommand("exec SearchVolume @High, @TankId, @Volume OUT", high, tankId, volume);
                    tank.VolumeMax = Double.Parse(volume.SqlValue.ToString());

                    tankRepository.Update(tank);
                    unitOfWork.Commit();

                    rs = true;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public MTank GetTankByID(int id)
        {
            return tankRepository.GetMany(tl => tl.TankId == id).FirstOrDefault();
        }

        public MTank FindTankById(int id)
        {
            return tankRepository.GetById(id);
        }

        public MTank FindTankById(int tankId, byte wareHouseCode)
        {
            return tankRepository.GetMany(tl => tl.TankId == tankId && tl.WareHouseCode == wareHouseCode).FirstOrDefault();
        }

        public IEnumerable<MTankLive> GetNewestTankLive()
        {
            return tankLiveRepository.GetAll()
                .OrderByDescending(tl => tl.InsertDate);
        }


        public IEnumerable<MLiveDataArm> GetNewestLiveDataArm()
        {
            return liveDataArmRepository.GetAll()
                .OrderByDescending(tl => tl.TimeLog);
        }

        public MTankLive GetNewestTankLive(int tankId)
        {
            return tankLiveRepository.GetMany(tl => tl.TankId == tankId)
                .OrderByDescending(tl => tl.TankId).FirstOrDefault();
        }

        public MTankLive GetNewestTankLive(int tankId, byte wareHouseCode)
        {
            return tankLiveRepository.GetMany(tl => tl.TankId == tankId && tl.WareHouseCode == wareHouseCode)
                .OrderByDescending(tl => tl.TankId).FirstOrDefault();
        }

        public bool CheckDataServerRunning()
        {
            var rs = true;
            var logTime = GetNewestTankLive().Max(tl => tl.InsertDate);

            if (logTime < DateTime.Now.AddSeconds(-Constants.CHECK_DATA_SERVER_TIME))
            {
                rs = false;
            }

            return rs;
        }

        public IEnumerable<MTankManual> GetAllTankManual()
        {
            return tankManualRepository.GetMany(tm => tm.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(tm => tm.MTank.TankName).ThenByDescending(tm => tm.InsertDate);
        }

        public IEnumerable<MTankManual> GetAllTankManual(int tankId)
        {
            return tankManualRepository.GetMany(tm => (tm.TankId == tankId) && tm.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(tm => tm.InsertDate);
        }

        public IEnumerable<MTankManual> GetTankManualByTime(int? tankId, DateTime? startDate, DateTime? endDate)
        {
            IEnumerable<MTankManual> rs = null;

            if (tankId == null)
            {
                if (startDate == null && endDate == null)
                {
                    rs = GetAllTankManual();
                }
                else if (startDate == null)
                {
                    rs = tankManualRepository
                    .GetMany(tl => tl.InsertDate <= endDate)
                    .OrderBy(tl => tl.MTank.TankName)
                    .ThenByDescending(tl => tl.InsertDate);
                }
                else if (endDate == null)
                {
                    rs = tankManualRepository
                    .GetMany(tl => startDate <= tl.InsertDate)
                    .OrderBy(tl => tl.MTank.TankName)
                    .ThenByDescending(tl => tl.InsertDate);
                }
                else
                {
                    rs = tankManualRepository
                    .GetMany(tl => (startDate <= tl.InsertDate) && (tl.InsertDate <= endDate))
                    .OrderBy(tl => tl.MTank.TankName)
                    .ThenByDescending(tl => tl.InsertDate);
                }
            }
            else
            {
                if (startDate == null && endDate == null)
                {
                    rs = GetAllTankManual(tankId.Value);
                }
                else if (startDate == null)
                {
                    rs = tankManualRepository
                    .GetMany(tl => (tankId == tl.TankId) && (tl.InsertDate <= endDate))
                    .OrderByDescending(tl => tl.InsertDate);
                }
                else if (endDate == null)
                {
                    rs = tankManualRepository
                    .GetMany(tl => (tankId == tl.TankId) && (startDate <= tl.InsertDate))
                    .OrderByDescending(tl => tl.InsertDate);
                }
                else
                {
                    rs = tankManualRepository
                    .GetMany(tl => (tankId == tl.TankId) && (startDate <= tl.InsertDate) && (tl.InsertDate <= endDate))
                    .OrderByDescending(tl => tl.InsertDate);
                }
            }

            return rs;
        }

        public MTankManual GetTankManual(int id)
        {
            return tankManualRepository.Get(tm => tm.Id == id);
        }

        public bool CreateTankManual(MTankManual tankManual)
        {
            var rs = false;

            try
            {
                var tankLog = tankLogRepository
                    .GetMany(t => t.TankId == (tankManual.TankId) && (t.InsertDate < tankManual.InsertDate))
                    .OrderByDescending(t => t.InsertDate).FirstOrDefault();

                if (tankLog != null)
                {
                    tankManual.LogWaterLevel = tankLog.WaterLevel;
                    tankManual.LogTotalLevel = tankLog.TotalLevel;
                    tankManual.LogAvgTemperature = tankLog.AvgTemperature;
                    tankManual.LogInsertDate = tankLog.InsertDate;
                }

                using (TransactionScope ts = new TransactionScope())
                {
                    tankManual.MTank = null;
                    tankManual.UpdateUser = tankManual.InsertUser;
                    tankManualRepository.Add(tankManual);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_CREATE_TANK_MANUAL, tankManual.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool UpdateTankManual(MTankManual tankManual)
        {
            var rs = false;

            try
            {
                var tankLog = tankLogRepository
                    .GetMany(t => t.TankId == (tankManual.TankId) && (t.InsertDate < tankManual.InsertDate))
                    .OrderByDescending(t => t.InsertDate).FirstOrDefault();

                if (tankLog != null)
                {
                    tankManual.LogWaterLevel = tankLog.WaterLevel;
                    tankManual.LogTotalLevel = tankLog.TotalLevel;
                    tankManual.LogAvgTemperature = tankLog.AvgTemperature;
                    tankManual.LogInsertDate = tankLog.InsertDate;
                }

                using (TransactionScope ts = new TransactionScope())
                {
                    tankManualRepository.Update(tankManual);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_UPDATE_TANK_MANUAL, tankManual.UpdateUser);
            }
            catch { }

            return rs;
        }

        public bool DeleteTankManual(MTankManual tankManual)
        {
            var rs = false;

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    tankManual.DeleteFlg = Constants.FLAG_ON;
                    tankManualRepository.Update(tankManual);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_DELETE_TANK_MANUAL, tankManual.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public MTankLog GetTankLogByTime(int tankId, DateTime date)
        {
            var offsetDate = date.AddMinutes(Constants.MINUTE_DATALOG_OFFSET);
            var rs = tankLogRepository.GetMany(tl => (tl.TankId == tankId) && (tl.InsertDate <= date)
                    && (tl.InsertDate >= offsetDate)).OrderByDescending(tl => tl.InsertDate).FirstOrDefault();

            if (rs == null)
            {
                rs = new MTankLog();
            }

            return rs;
        }


        public MTankLog GetTankLogByTime(byte wareHouse, int tankId, DateTime date)
        {
            var offsetDate = date.AddMinutes(Constants.MINUTE_DATALOG_OFFSET);
            var rs = tankLogRepository.GetMany(tl => (tl.TankId == tankId) && (tl.InsertDate <= date) && (tl.WareHouseCode == wareHouse)
                    && (tl.InsertDate >= offsetDate)).OrderByDescending(tl => tl.InsertDate).FirstOrDefault();

            if (rs == null)
            {
                rs = new MTankLog();
            }

            return rs;
        }
        public IEnumerable<MTankLog> GetAllDataTankLog(byte wareHouse, int productId, DateTime? startDate, DateTime? endDate)
        {
            return tankLogRepository.GetMany(tl => tl.ProductId == productId && tl.WareHouseCode == wareHouse && tl.InsertDate > startDate && tl.InsertDate < endDate).OrderByDescending(tl => tl.InsertDate);
        }
        public IEnumerable<MTankLog> GetAllTankLog()
        {
            return tankLogRepository.GetAll().OrderBy(tl => tl.MTank.TankName).ThenByDescending(tl => tl.InsertDate);
        }
        public IEnumerable<MTankLog> GetAllTankLog(int tankId)
        {
            return tankLogRepository.GetMany(tl => tl.TankId == tankId).OrderByDescending(tl => tl.InsertDate);
        }

        public IEnumerable<MTankLog> GetAllTankLog(byte warehouseCode, int tankId)
        {
            return tankLogRepository.GetMany(tl => tl.TankId == tankId && tl.WareHouseCode == warehouseCode).OrderByDescending(tl => tl.InsertDate);
        }

        public IEnumerable<MTankLog> GetTankLogByTime(byte? wareHouseCode, int? tankId, DateTime? startDate, DateTime? endDate)
        {
            IEnumerable<MTankLog> rs = null;

            if (wareHouseCode != null)
            {
                if (tankId == null)
                {
                    if (startDate == null && endDate == null)
                    {
                        rs = GetAllTankLog();
                    }
                    else if (startDate == null)
                    {
                        rs = tankLogRepository
                        .GetMany(tl => tl.InsertDate <= endDate)
                        .OrderBy(tl => tl.MTank.TankName)
                        .ThenByDescending(tl => tl.InsertDate);
                    }
                    else if (endDate == null)
                    {
                        rs = tankLogRepository
                        .GetMany(tl => startDate <= tl.InsertDate)
                        .OrderBy(tl => tl.MTank.TankName)
                        .ThenByDescending(tl => tl.InsertDate);
                    }
                    else
                    {
                        rs = tankLogRepository
                        .GetMany(tl => (startDate <= tl.InsertDate) && (tl.InsertDate <= endDate))
                        .OrderBy(tl => tl.MTank.TankName)
                        .ThenByDescending(tl => tl.InsertDate);
                    }
                }
                else
                {
                    if (startDate == null && endDate == null)
                    {
                        rs = GetAllTankLog(tankId.Value);
                    }
                    else if (startDate == null)
                    {
                        rs = tankLogRepository
                        .GetMany(tl => (tankId == tl.TankId) && (tl.InsertDate <= endDate))
                        .OrderByDescending(tl => tl.InsertDate);
                    }
                    else if (endDate == null)
                    {
                        rs = tankLogRepository
                        .GetMany(tl => (tankId == tl.TankId) && (startDate <= tl.InsertDate))
                        .OrderByDescending(tl => tl.InsertDate);
                    }
                    else
                    {
                        rs = tankLogRepository
                        .GetMany(tl => (tankId == tl.TankId) && (startDate <= tl.InsertDate) && (tl.InsertDate <= endDate) && (tl.WareHouseCode == wareHouseCode))
                        .OrderByDescending(tl => tl.InsertDate);
                    }
                }

            }
            else
            {
                rs = tankLogRepository
                        .GetMany(tl => (tankId == tl.TankId) && (startDate <= tl.InsertDate) && (tl.InsertDate <= endDate) && (tl.WareHouseCode == wareHouseCode))
                        .OrderByDescending(tl => tl.InsertDate);
            }

            return rs;
        }

        public IEnumerable<MTankLog> GetTankLogByTime(int? tankId, DateTime? startDate, DateTime? endDate)
        {
            IEnumerable<MTankLog> rs = null;

            if (tankId == null)
            {
                if (startDate == null && endDate == null)
                {
                    rs = GetAllTankLog();
                }
                else if (startDate == null)
                {
                    rs = tankLogRepository
                    .GetMany(tl => tl.InsertDate <= endDate)
                    .OrderBy(tl => tl.MTank.TankName)
                    .ThenByDescending(tl => tl.InsertDate);
                }
                else if (endDate == null)
                {
                    rs = tankLogRepository
                    .GetMany(tl => startDate <= tl.InsertDate)
                    .OrderBy(tl => tl.MTank.TankName)
                    .ThenByDescending(tl => tl.InsertDate);
                }
                else
                {
                    rs = tankLogRepository
                    .GetMany(tl => (startDate <= tl.InsertDate) && (tl.InsertDate <= endDate))
                    .OrderBy(tl => tl.MTank.TankName)
                    .ThenByDescending(tl => tl.InsertDate);
                }
            }
            else
            {
                if (startDate == null && endDate == null)
                {
                    rs = GetAllTankLog(tankId.Value);
                }
                else if (startDate == null)
                {
                    rs = tankLogRepository
                    .GetMany(tl => (tankId == tl.TankId) && (tl.InsertDate <= endDate))
                    .OrderByDescending(tl => tl.InsertDate);
                }
                else if (endDate == null)
                {
                    rs = tankLogRepository
                    .GetMany(tl => (tankId == tl.TankId) && (startDate <= tl.InsertDate))
                    .OrderByDescending(tl => tl.InsertDate);
                }
                else
                {
                    rs = tankLogRepository
                    .GetMany(tl => (tankId == tl.TankId) && (startDate <= tl.InsertDate) && (tl.InsertDate <= endDate))
                    .OrderByDescending(tl => tl.InsertDate);
                }
            }



            return rs;
        }

        public double GetTankTurnOverNumber(int id, DateTime startDate, DateTime endDate)
        {
            double rs = 0;

            var tank = GetTankByID(id);
            var volume = GetTankExport(id, startDate, endDate);

            if (tank.VolumeMax != null)
            {
                rs = Math.Round((double)(volume / tank.VolumeMax), 3);
            }

            return rs;
        }

        public double GetTankExport(int id, DateTime startDate, DateTime endDate)
        {
            double volume = 0;
            var tank = GetTankByID(id);

            try
            {
                using (var context = new PetroBMContext())
                {
                    var start = new SqlParameter("@StartDate", startDate);
                    var end = new SqlParameter("@EndDate", endDate);
                    var tankId = new SqlParameter("@TankId", id);

                    volume = context.Database.SqlQuery<double>("select dbo.SeachProductOutByTankId(@TankId, @StartDate, @EndDate)"
                        , tankId, start, end).First();
                }
            }
            catch { }

            return volume;
        }

        public double GetUsagePerformanceOfTank(int id, DateTime startDate, DateTime endDate)
        {
            double rs = 0;

            var tank = FindTankById(id);
            var volume = GetAvgProductVolumeInTank(id, startDate, endDate);

            if (tank.VolumeMax != null)
            {
                rs = Math.Round((double)(volume / tank.VolumeMax), 4) * 100;
            }

            return rs;
        }

        public double GetAvgProductVolumeInTank(int id, DateTime startDate, DateTime endDate)
        {
            double volume = 0;
            var tank = FindTankById(id);

            try
            {
                using (var context = new PetroBMContext())
                {
                    var start = new SqlParameter("@StartDate", startDate);
                    var end = new SqlParameter("@EndDate", endDate);
                    var tankId = new SqlParameter("@TankId", id);

                    volume = context.Database.SqlQuery<double>("select dbo.SeachAvgProductVolume(@TankId, @StartDate, @EndDate)"
                        , tankId, start, end).First();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return volume;
        }

        public double GetRepositoryTurnOverNumber(DateTime startDate, DateTime endDate)
        {
            double rs = 0;

            try
            {
                var volume = GetRepositoryExport(startDate, endDate);
                var volumeMax = GetRepositoryVolume();

                rs = Math.Round((double)(volume / volumeMax), 3);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public double GetRepositoryExport(DateTime startDate, DateTime endDate)
        {
            double volume = 0;

            foreach (var tank in GetAllTank())
            {
                volume += GetTankExport(tank.TankId, startDate, endDate);
            }

            return volume;
        }

        public double GetUsagePerformanceOfRepository(DateTime startDate, DateTime endDate)
        {
            double rs = 0;
            try
            {
                var volume = GetAvgProductVolumeInRepository(startDate, endDate);
                var volumeMax = GetRepositoryVolume();

                rs = Math.Round((double)(volume / volumeMax), 4) * 100;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public double GetAvgProductVolumeInRepository(DateTime startDate, DateTime endDate)
        {
            double volume = 0;

            foreach (var tank in GetAllTank())
            {
                volume += GetAvgProductVolumeInTank(tank.TankId, startDate, endDate);
            }

            return volume;
        }

        public double GetRepositoryVolume()
        {
            double volumeMax = 0;

            foreach (var tank in GetAllTank())
            {
                if (tank.VolumeMax != null)
                {
                    volumeMax += tank.VolumeMax.Value;
                }
            }

            return volumeMax;
        }

        public double GetProductOut(byte wareHouseCode,int productId, DateTime startDate, DateTime endDate)
        {
            double volume = 0;

            try
            {
                using (var context = new PetroBMContext())
                {
                    var warehouse = new SqlParameter("@WareHouseCode", wareHouseCode);
                    var start = new SqlParameter("@StartDate", startDate);
                    var end = new SqlParameter("@EndDate", endDate);
                    var product = new SqlParameter("@ProductId", productId);

                    volume = context.Database.SqlQuery<double>("select dbo.SeachProductOutByProductId(@WareHouseCode , @ProductId, @StartDate, @EndDate)",
                        warehouse, product, start, end).First();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return volume;
        }

        public IEnumerable<MTank> GetAllTankByProduct(int productId)
        {
            return tankRepository.GetMany(p => p.ProductId == productId).Where(t => (t.TankId != Constants.TEC_CC_ID) && (t.DeleteFlg == Constants.FLAG_OFF))
                .OrderByDescending(tm => tm.InsertDate);
        }

        public IEnumerable<MTankDensity> GetAllTankDensity(int tankId)
        {
            return tankDensityRepository.GetMany(td => td.TankId == tankId).OrderByDescending(td => td.InsertDate);
        }

        public IEnumerable<MTankDensity> GetAllTankDensity(int tankId, byte wareHouseCode)
        {
            return tankDensityRepository.GetMany(td => td.TankId == tankId && td.WareHouseCode == wareHouseCode).OrderByDescending(td => td.InsertDate);
        }

        public bool CreateTankDensity(MTankDensity tankDensity)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    tankDensityRepository.Add(tankDensity);
                    unitOfWork.Commit();

                    var tank = FindTankById(tankDensity.TankId, tankDensity.WareHouseCode);
                    tank.Density = tankDensity.Density;
                    UpdateTank(tank);

                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_DENSITY_CREATE, tankDensity.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public IEnumerable<MBarem> GetBarem(int tankId)
        {
            return baremRepository.GetMany(br => br.TankId == tankId).OrderBy(br => br.High);
        }

        public IEnumerable<MBarem> GetBarem(int tankId, byte wareHouse)
        {
            return baremRepository.GetMany(br => br.TankId == tankId && br.WareHouseCode == wareHouse).OrderBy(br => br.High);
        }

        public MBarem GetBaremByHigh(int tankId, float high)
        {
            return baremRepository.Get(br => (br.TankId == tankId) && (br.High == high));
        }

        public MBarem GetBaremByHigh(byte wareHouseCode, int tankId, float high)
        {
            return baremRepository.Get(br => (br.TankId == tankId) && (br.High == high) && (br.WareHouseCode == wareHouseCode));
        }

        public bool CreateBarem(MBarem barem)
        {
            var rs = false;

            try
            {
                baremRepository.Add(barem);
                unitOfWork.Commit();

                rs = true;

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_BAREM_CREATE, barem.InsertUser);

                ResetHighMax(barem.TankId, barem.WareHouseCode);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool UpdateBarem(MBarem barem)
        {
            var rs = false;

            try
            {
                baremRepository.Update(barem);
                unitOfWork.Commit();

                rs = true;

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_BAREM_UPDATE, barem.UpdateUser);

                ResetHighMax(barem.TankId, barem.WareHouseCode);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool DeleteBarem(int tankId, IEnumerable<float> highs, string user)
        {
            var rs = false;

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var barems = new List<MBarem>();

                    foreach (var high in highs)
                    {
                        var barem = GetBaremByHigh(tankId, high);
                        barems.Add(barem);
                    }

                    baremRepository.DeleteRange(barems);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_BAREM_DELETE, user);

                //ResetHighMax(barem.TankId, barem.WareHouseCode);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool ImportBarem(byte wareHouseCode, int tankId, HttpPostedFileBase file, string user)
        {
            var rs = false;

            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    var path = Path.Combine(HttpContext.Current.Server.MapPath(Constants.FILE_PATH), fileName);
                    file.SaveAs(path);

                    //  var data = ExcelUtil.ReadFromExcelfile(path);
                    var data = ExcelUtil.CommonReadFromExcelfile(path, Constants.Import_Column_Barem);
                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 15, 0)))
                    {
                        //var baremList = GetBarem(tankId, wareHouseCode);
                        //baremRepository.DeleteRange(baremList.ToList());
                        rs = DeleteBarem_By_TankId_WareHouseCode(tankId, wareHouseCode);

                        var barems = new List<MBarem>();

                        foreach (System.Data.DataRow row in data.Rows)
                        {
                            if (/*byte.Parse(row.ItemArray[0].ToString()) == wareHouseCode*/int.Parse(row.ItemArray[0].ToString()) == tankId)
                            {
                                var barem = new MBarem();
                                barem.TankId = tankId;
                                barem.WareHouseCode = wareHouseCode;
                                barem.High = float.Parse(row.ItemArray[1].ToString());
                                barem.Volume = float.Parse(row.ItemArray[2].ToString());
                                barem.InsertUser = user;
                                barem.UpdateUser = user;
                                barem.InsertDate = DateTime.Now;
                                barem.UpdateDate = DateTime.Now;
                                barem.VersionNo = Constants.VERSION_START;
                                barem.DeleteFlg = Constants.FLAG_OFF;
                                barems.Add(barem);
                            }
                        }

                        baremRepository.AddRange(barems);
                        unitOfWork.Commit();

                        ts.Complete();
                        rs = true;
                    }

                    // Log event
                    eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                        Constants.EVENT_CONFIG_BAREM_IMPORT, user);

                    //ResetHighMax(tankId);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        private void ResetHighMax(int tankId, byte warehousecode)
        {
            var tank = GetAllTank().Where(x => x.TankId == tankId && x.WareHouseCode == warehousecode).FirstOrDefault();
            var barem = GetBarem(tankId, warehousecode).OrderByDescending(br => br.High).FirstOrDefault();

            if (barem == null)
            {
                tank.HighMax = 0;
                tankRepository.Update(tank);
                unitOfWork.Commit();
            }
            else if (tank.HighMax > barem.High)
            {
                tank.HighMax = barem.High;
                tankRepository.Update(tank);
                unitOfWork.Commit();
            }
        }

        public double SearchVolume(int id, float high)
        {
            double rs = 0;
            var tank = GetTankByID(id);

            try
            {
                using (var context = new PetroBMContext())
                {
                    var highParam = new SqlParameter("@High", high);
                    var tankId = new SqlParameter("@TankId", id);
                    var volume = new SqlParameter("@Volume", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var warehousecode = new SqlParameter("@WareHouseCode", tank.WareHouseCode);

                    context.Database.ExecuteSqlCommand("exec SearchVolume @High, @TankId, @Volume OUT, @WareHouseCode", highParam, tankId, volume, warehousecode);
                    rs = Double.Parse(volume.SqlValue.ToString());
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }


        public bool DeleteBarem_By_TankId_WareHouseCode(int tankid, byte warehousecode)
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
                            cmd.CommandText = "Delete_Barem_By_TankId_WareHouseCode";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("TankId", tankid);
                            SqlParameter param2 = new SqlParameter("WareHouseCode", warehousecode);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
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

        public float SearchVCF(float d15, float t)
        {
            double K, L, M;
            float vcfff = 0;

            K = d15 * 500;
            K = (int)(K + 0.5);
            K = K * 2;
            L = t * 4;
            L = (int)(L + 0.5);
            L = L / 4 - 15;

            if (K < 770.5)
            {
                M = 346.4228 / (K * K) + 0.4388 / K;
                M = (int)((M * 10000000) + 0.5);
                M = M / 10000000;
            }
            else
            {
                if ((K >= 770.5) && (K < 787.5))
                {
                    M = 2680.3206 / (K * K) - 0.003363;
                    M = (int)((M * 10000000) + 0.5);
                    M = M / 10000000;
                }
                else
                {
                    if ((K >= 787.5) && (K < 838.5))
                    {
                        M = 594.5418 / (K * K) + 0 / K;
                        M = (int)((M * 10000000) + 0.5);
                        M = M / 10000000;
                    }
                    else
                    {
                        M = 186.9696 / (K * K) + 0.4862 / K;
                        M = ((int)((M * 10000000) + 0.5));
                        M = M / 10000000;
                    }
                }
            }

            if ((d15 == 0) && (t == 0))
            {
                vcfff = 0;
            }
            else
            {
                double temp = 0;
                temp = -L * M * (1 + 0.8 * L * M);
                temp = (int)((temp * 100000000 + 0.5));
                temp = temp / 100000000;
                vcfff = (float)Math.Exp(temp);
                vcfff = (int)(vcfff * 10000 + 0.5);
                vcfff = vcfff / 10000;
            }

            return vcfff;
        }

        public IEnumerable<MTank> GetAllTankByWareHouseCode(byte warehousecode)
        {
            return tankRepository.GetAll().Where(p => p.WareHouseCode == warehousecode).OrderBy(p => p.TankId);
        }

        public IEnumerable<MTank> GetTankIdByTankName(string tankname)
        {
            return tankRepository.GetAll().Where(p => p.TankName == tankname).OrderBy(p => p.TankId);
        }

        public int GetMaxTankId_By_WareHouseCode(byte wareHouseCode)
        {
            if (tankRepository.GetAll().Where(p => p.WareHouseCode == wareHouseCode).Count() == 0)
            {
                return 0;
            }
            else
            {
                return tankRepository.GetAll().Where(p => p.WareHouseCode == wareHouseCode).Max(x => x.TankId);
            }

        }
        public List<MTankLive> GetTankLive_Greater_Time(byte warehousecode, int tankid, DateTime greaterTime)
        {
            List<MTankLive> lst = new List<MTankLive>();

            try
            {

                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "GetTankLive_Greater_Time";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("WareHouseCode", warehousecode);
                            SqlParameter param2 = new SqlParameter("TankId", tankid);
                            SqlParameter param3 = new SqlParameter("Time", greaterTime);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            SqlDataReader reader = cmd.ExecuteReader();
                            var it = new MTankLive();
                            while (reader.Read())
                            {

                                it.WareHouseCode = byte.Parse(reader["WareHouseCode"].ToString());
                                it.TankId = int.Parse(reader["TankId"].ToString());
                                it.InsertDate = DateTime.Parse(reader["InsertDate"].ToString());
                                it.WaterLevel = float.Parse(reader["WaterLevel"].ToString());
                                it.ProductLevel = float.Parse(reader["ProductLevel"].ToString());
                                it.TotalLevel = float.Parse(reader["TotalLevel"].ToString());
                                it.WaterVolume = double.Parse(reader["WaterVolume"].ToString());
                                it.ProductVolume = double.Parse(reader["ProductVolume"].ToString());
                                it.TotalVolume = double.Parse(reader["TotalVolume"].ToString());
                                it.TankEmpty = double.Parse(reader["TankEmpty"].ToString());
                                it.AvgTemperature = float.Parse(reader["AvgTemperature"].ToString());
                                it.Temperature1 = float.Parse(reader["Temperature1"].ToString());
                                it.Temperature2 = float.Parse(reader["Temperature2"].ToString());
                                it.Temperature3 = float.Parse(reader["Temperature3"].ToString());
                                it.Temperature4 = float.Parse(reader["Temperature4"].ToString());
                                it.Temperature5 = float.Parse(reader["Temperature5"].ToString());
                                it.Temperature6 = float.Parse(reader["Temperature6"].ToString());
                                it.Temperature7 = float.Parse(reader["Temperature7"].ToString());
                                it.Temperature8 = float.Parse(reader["Temperature8"].ToString());
                                it.Temperature9 = float.Parse(reader["Temperature9"].ToString());
                                it.Temperature10 = float.Parse(reader["Temperature10"].ToString());
                                it.LevelRate = double.Parse(reader["LevelRate"].ToString());
                                it.FlowRate = double.Parse(reader["FlowRate"].ToString());
                                it.AvailableVolume = double.Parse(reader["AvailableVolume"].ToString());
                                it.Mass = double.Parse(reader["Mass"].ToString());
                                it.MassRate = double.Parse(reader["MassRate"].ToString());
                                it.ProductVolume15 = double.Parse(reader["ProductVolume15"].ToString());
                                it.VCF = float.Parse(reader["VCF"].ToString());
                                it.WCF = float.Parse(reader["WCF"].ToString());

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