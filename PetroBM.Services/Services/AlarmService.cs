using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using PetroBM.Domain.Entities;
using PetroBM.Data.Repositories;
using PetroBM.Data.Infrastructure;
using PetroBM.Common.Util;
using System.Data;
using System.Data.SqlClient;
using PetroBM.Data;

namespace PetroBM.Services.Services
{
    public interface IAlarmService
    {
        IEnumerable<MAlarm> GetAllAlarm();
        IEnumerable<MAlarm> GetTopAlarm(int count);

        IEnumerable<MAlarm> GetTopAlarm(int count,string userName);

        MAlarm GetNewestAlarm(int tankId);

        MAlarm GetNewestAlarm(int tankId,byte wareHouse);
        MAlarm GetAlarmById(int alarmId);
        IEnumerable<MAlarm> GetAlarmByTime(DateTime? startDate, DateTime? endDate, int? tankId, int? alarmTypeId, int? warehousecode);
        IEnumerable<MAlarm> GetAlarmByTimeAndWareHouse(DateTime? startDate, DateTime? endDate,int? alarmTypeId, byte warehousecode);
        IEnumerable<MAlarm> GetErrorAlarm(DateTime startDate, DateTime endDate);
        IEnumerable<MAlarm> GetWarningAlarm(DateTime startDate, DateTime endDate);
        MAlarm GetAlarmNotConfirmed();
        bool ConfirmAlarm(MAlarm alarm);
        bool SolveAlarm(MAlarm alarm);

        IEnumerable<MAlarmType> GetAllAlarmType();
        MAlarmType GetAlarmTypeById(int id);
        bool UpdateSoundAlarm(MAlarmType alarmType, string user);
    }

    public class AlarmService : IAlarmService
    {
        private readonly IAlarmRepository alarmRepository;
        private readonly IAlarmTypeRepository alarmTypeRepository;
        private readonly IEventService eventService;
        private readonly IUnitOfWork unitOfWork;

        public AlarmService(IAlarmRepository alarmRepository, IAlarmTypeRepository alarmTypeRepository
            , IEventService eventService, IUnitOfWork unitOfWork)
        {
            this.alarmRepository = alarmRepository;
            this.alarmTypeRepository = alarmTypeRepository;
            this.eventService = eventService;
            this.unitOfWork = unitOfWork;
        }

        #region IEventService Members

        public IEnumerable<MAlarm> GetAllAlarm()
        {
            return alarmRepository.GetAll().OrderByDescending(ev => ev.InsertDate);
        }

        public IEnumerable<MAlarm> GetTopAlarm(int count)
        {
            //var date = DateTime.Now.AddDays(Constants.DAYS_ALARM_OFFSET);
            //return GetAllAlarm().Where(al => al.InsertDate >= date).Take(count);
            List<MAlarm> topAlarm = alarmRepository.GetTopAlarm(count);

            return topAlarm;
        }

        public MAlarm GetNewestAlarm(int tankId)
        {
            //var date = DateTime.Now.AddDays(Constants.DAYS_ALARM_OFFSET);
            //return alarmRepository.GetMany(al => (al.TankId == tankId) && (!al.ConfirmFlag)
            //    && (al.InsertDate >= date))
            //    .OrderByDescending(al => al.InsertDate).FirstOrDefault();

            MAlarm newestAlarm = alarmRepository.GetNewestAlarm(tankId);

            return newestAlarm;
        }

        public MAlarm GetNewestAlarm(int tankId,byte wareHouse)
        {
            //var date = DateTime.Now.AddDays(Constants.DAYS_ALARM_OFFSET);
            //return alarmRepository.GetMany(al => (al.TankId == tankId) && (!al.ConfirmFlag)
            //    && (al.InsertDate >= date))
            //    .OrderByDescending(al => al.InsertDate).FirstOrDefault();

            MAlarm newestAlarm = alarmRepository.GetNewestAlarm(tankId, wareHouse);

            return newestAlarm;
        }

        public MAlarm GetAlarmById(int alarmId)
        {
            return alarmRepository.GetById(alarmId);
        }

        public IEnumerable<MAlarm> GetAlarmByTime(DateTime? startDate, DateTime? endDate, int? tankId, int? alarmTypeId, int? warehousecode)
        {
            IEnumerable<MAlarm> rs = null;

            if (tankId == null && alarmTypeId == null)
            {
                if (startDate == null && endDate == null)
                {
                    rs = GetAllAlarm();
                }
                else if (startDate == null)
                {
                    rs = alarmRepository.GetMany(al => al.InsertDate < endDate)
                    .OrderByDescending(al => al.InsertDate);
                }
                else if (endDate == null)
                {
                    rs = alarmRepository.GetMany(al => startDate < al.InsertDate)
                    .OrderByDescending(al => al.InsertDate);
                }
                else
                {
                    rs = alarmRepository.GetMany(al => (startDate < al.InsertDate) && (al.InsertDate < endDate) && (al.WareHouseCode == warehousecode))
                    .OrderByDescending(al => al.InsertDate);
                }
            }
            else if (tankId == null)
            {
                if (startDate == null && endDate == null)
                {
                    rs = alarmRepository.GetMany(al => al.MAlarmType.TypeId == alarmTypeId)
                        .OrderByDescending(ev => ev.InsertDate);
                }
                else if (startDate == null)
                {
                    rs = alarmRepository.GetMany(al => (al.MAlarmType.TypeId == alarmTypeId) && (al.InsertDate < endDate))
                    .OrderByDescending(al => al.InsertDate);
                }
                else if (endDate == null)
                {
                    rs = alarmRepository.GetMany(al => (al.MAlarmType.TypeId == alarmTypeId) && (startDate < al.InsertDate))
                    .OrderByDescending(al => al.InsertDate);
                }
                else
                {
                    rs = alarmRepository.GetMany(al => (al.MAlarmType.TypeId == alarmTypeId)
                    && (startDate < al.InsertDate) && (al.InsertDate < endDate) &&(al.WareHouseCode == warehousecode))
                    .OrderByDescending(al => al.InsertDate);
                }
            }
            else if (alarmTypeId == null)
            {
                if (startDate == null && endDate == null)
                {
                    rs = alarmRepository.GetMany(al => al.TankId == tankId)
                        .OrderByDescending(ev => ev.InsertDate);
                }
                else if (startDate == null)
                {
                    rs = alarmRepository.GetMany(al => (al.TankId == tankId) && (al.InsertDate < endDate))
                    .OrderByDescending(al => al.InsertDate);
                }
                else if (endDate == null)
                {
                    rs = alarmRepository.GetMany(al => (al.TankId == tankId) && (startDate < al.InsertDate))
                    .OrderByDescending(al => al.InsertDate);
                }
                else
                {
                    rs = alarmRepository.GetMany(al => (al.TankId == tankId)
                    && (startDate < al.InsertDate) && (al.InsertDate < endDate) && (al.WareHouseCode == warehousecode))
                    .OrderByDescending(al => al.InsertDate);
                }
            }
            else
            {
                if (startDate == null && endDate == null)
                {
                    rs = alarmRepository.GetMany(al => (al.TankId == tankId) && (al.MAlarmType.TypeId == alarmTypeId))
                        .OrderByDescending(ev => ev.InsertDate);
                }
                else if (startDate == null)
                {
                    rs = alarmRepository.GetMany(al => (al.TankId == tankId) && (al.MAlarmType.TypeId == alarmTypeId)
                    && (al.InsertDate < endDate))
                    .OrderByDescending(al => al.InsertDate);
                }
                else if (endDate == null)
                {
                    rs = alarmRepository.GetMany(al => (al.TankId == tankId) && (al.MAlarmType.TypeId == alarmTypeId)
                    && (startDate < al.InsertDate))
                    .OrderByDescending(al => al.InsertDate);
                }
                else
                {
                    rs = alarmRepository.GetMany(al => (al.TankId == tankId) && (al.MAlarmType.TypeId == alarmTypeId)
                    && (startDate < al.InsertDate) && (al.InsertDate < endDate) && (al.WareHouseCode == warehousecode))
                    .OrderByDescending(al => al.InsertDate);
                }
            }

            return rs;
        }

        public MAlarm GetAlarmNotConfirmed()
        {
            var date = DateTime.Now.AddDays(Constants.DAYS_ALARM_OFFSET);
            return alarmRepository.GetMany(al => (!al.ConfirmFlag) && (al.InsertDate >= date))
                .OrderByDescending(al => al.InsertDate).FirstOrDefault();
        }

        public bool ConfirmAlarm(MAlarm alarm)
        {
            bool rs = false;
            try
            {
                alarm.ConfirmFlag = true;
                alarm.ConfirmDate = DateTime.Now;
                alarmRepository.Update(alarm);
                SaveAlarm();

                rs = true;

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIRM_ALARM, alarm.ConfirmUser);
            }
            catch { }

            return rs;
        }

        public bool SolveAlarm(MAlarm alarm)
        {
            bool rs = false;
            try
            {
                if (alarm.ConfirmFlag)
                {
                    alarm.SolveFlag = true;
                    alarm.SolveDate = DateTime.Now;
                    alarmRepository.Update(alarm);
                    SaveAlarm();

                    rs = true;

                    // Log event
                    eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                        Constants.EVENT_SOLVE_ALARM, alarm.SolveUser);
                }

            }
            catch { }

            return rs;
        }

        public IEnumerable<MAlarmType> GetAllAlarmType()
        {
            return alarmTypeRepository.GetAll().OrderByDescending(x=>x.TypeId);
        }

        public MAlarmType GetAlarmTypeById(int id)
        {
            return alarmTypeRepository.GetById(id);
        }

        public bool UpdateSoundAlarm(MAlarmType alarmType, string user)
        {
            var rs = false;

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    alarmTypeRepository.Update(alarmType);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_ALARM_SOUND, user);
            }
            catch { }

            return rs;
        }

        private void SaveAlarm()
        {
            unitOfWork.Commit();
        }

        public IEnumerable<MAlarm> GetErrorAlarm(DateTime startDate, DateTime endDate)
        {
            return alarmRepository.GetMany(al => (startDate < al.InsertDate) && (al.InsertDate < endDate) && (al.TypeId == 1))
                .OrderByDescending(al => al.InsertDate);
        }

        public IEnumerable<MAlarm> GetWarningAlarm(DateTime startDate, DateTime endDate)
        {
            return alarmRepository.GetMany(al => (startDate < al.InsertDate) && (al.InsertDate < endDate) && (al.TypeId != 1))
                .OrderByDescending(al => al.InsertDate);
        }

        public IEnumerable<MAlarm> GetTopAlarm(int count, string userName)
        {
            List<MAlarm> topAlarm = new List<MAlarm>();
            var date = DateTime.Now.AddDays(Constants.DAYS_ALARM_OFFSET);// Thời gian lấy
            try
            {

                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "GetAlarm_TopAlarm_UserName";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("UserName", userName);
                            SqlParameter param2 = new SqlParameter("Count", count);
                            SqlParameter param3 = new SqlParameter("FromDate", date);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            SqlDataReader reader = cmd.ExecuteReader();
                          
                            while (reader.Read())
                            {
                                var it = new MAlarm();
                                it.AlarmId = int.Parse(reader["AlarmId"].ToString());
                                it.TypeId = int.Parse(reader["TypeId"].ToString());
                                it.WareHouseCode = byte.Parse(reader["WareHouseCode"].ToString());
                                it.TankId = string.IsNullOrEmpty(reader["TankId"].ToString()) ? (int?)null : int.Parse(reader["TankId"].ToString());
                                it.Issue = reader["Issue"].ToString();
                                it.Value = reader["Value"].ToString();
                                it.InsertDate = DateTime.Parse(reader["InsertDate"].ToString());
                                it.ConfirmFlag = bool.Parse(reader["ConfirmFlag"].ToString());
                                it.ConfirmUser =reader["ConfirmUser"].ToString();
                                it.ConfirmDate = string.IsNullOrEmpty(reader["ConfirmDate"].ToString()) ? (DateTime?)null : DateTime.Parse(reader["ConfirmDate"].ToString());
                                it.ConfirmComment = reader["ConfirmComment"].ToString();
                                it.SolveFlag = bool.Parse(reader["SolveFlag"].ToString());
                                it.SolveUser =reader["SolveUser"].ToString();
                                it.ConfirmDate = string.IsNullOrEmpty(reader["SolveDate"].ToString()) ? (DateTime?)null : DateTime.Parse(reader["SolveDate"].ToString());
                                it.SolveComment = reader["SolveComment"].ToString();
                                topAlarm.Add(it);
                            }
                        }
                        conn.Close();
                    }
                }

            }
            catch (Exception ex)
            {
            }

            return topAlarm;
        }

        public IEnumerable<MAlarm> GetAlarmByTimeAndWareHouse(DateTime? startDate, DateTime? endDate, int? alarmTypeId, byte warehousecode)
        {
            List<MAlarm> listAlarm = new List<MAlarm>();
            var date = DateTime.Now.AddDays(Constants.DAYS_ALARM_OFFSET);// Thời gian lấy
            try
            {

                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "GetAlarm_By_Time_WareHouse";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("FromDate", startDate);
                            SqlParameter param2 = new SqlParameter("ToDate", endDate);
                            SqlParameter param3 = new SqlParameter("AlarmTypeId", alarmTypeId??0);
                            SqlParameter param4 = new SqlParameter("WareHouseCode", warehousecode);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.Parameters.Add(param4);
                            SqlDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                var it = new MAlarm();
                                it.AlarmId = int.Parse(reader["AlarmId"].ToString());
                                it.TypeId = int.Parse(reader["TypeId"].ToString());
                                it.WareHouseCode = byte.Parse(reader["WareHouseCode"].ToString());
                                it.TankId = string.IsNullOrEmpty(reader["TankId"].ToString()) ? (int?)null : int.Parse(reader["TankId"].ToString());  
                                it.Issue = reader["Issue"].ToString();
                                it.Value = reader["Value"].ToString();
                                it.InsertDate = DateTime.Parse(reader["InsertDate"].ToString());
                                it.ConfirmFlag = bool.Parse(reader["ConfirmFlag"].ToString());
                                it.ConfirmUser = reader["ConfirmUser"].ToString();
                                it.ConfirmDate = string.IsNullOrEmpty(reader["ConfirmDate"].ToString()) ? (DateTime?)null : DateTime.Parse(reader["ConfirmDate"].ToString());
                                it.ConfirmComment = reader["ConfirmComment"].ToString();
                                it.SolveFlag = bool.Parse(reader["SolveFlag"].ToString());
                                it.SolveUser = reader["SolveUser"].ToString();
                                it.ConfirmDate = string.IsNullOrEmpty(reader["SolveDate"].ToString()) ? (DateTime?)null : DateTime.Parse(reader["SolveDate"].ToString());
                                it.SolveComment = reader["SolveComment"].ToString();
                                listAlarm.Add(it);
                            }
                        }
                        conn.Close();
                    }
                }

            }
            catch (Exception ex)
            {
            }

            return listAlarm;
        }
        #endregion
    }
}
