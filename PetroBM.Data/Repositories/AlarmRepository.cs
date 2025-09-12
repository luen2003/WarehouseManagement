using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetroBM.Common.Util;
using System.Linq.Expressions;

namespace PetroBM.Data.Repositories
{
    public interface IAlarmRepository : IRepository<MAlarm>
    {
        List<MAlarm> GetTopAlarm(int count);
        MAlarm GetNewestAlarm(int tankId);
        MAlarm GetNewestAlarm(int tankId, byte wareHouse);
    }

    public class AlarmRepository : RepositoryBase<MAlarm>, IAlarmRepository
    {
        public AlarmRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        public List<MAlarm> GetTopAlarm(int count)
        {
            List<MAlarm> alarms = new List<MAlarm>();
            var date = DateTime.Now.AddDays(Constants.DAYS_ALARM_OFFSET);
            if (dbSet.Any(al => al.InsertDate >= date))
            {
                alarms = dbSet.Where(al => al.InsertDate >= date).Take(count).ToList();
            }
            return alarms;
        }

        public MAlarm GetNewestAlarm(int tankId)
        {
            var date = DateTime.Now.AddDays(Constants.DAYS_ALARM_OFFSET);
            var alarm = dbSet.Where(al => (al.TankId == tankId) && (!al.ConfirmFlag)&& (al.InsertDate >= date))
                .OrderByDescending(al => al.InsertDate).FirstOrDefault();

            return alarm;
        }

        public MAlarm GetNewestAlarm(int tankId,byte wareHouse)
        {
            var date = DateTime.Now.AddDays(Constants.DAYS_ALARM_OFFSET);
            var alarm = dbSet.Where(al => (al.TankId == tankId && al.WareHouseCode == wareHouse) && (!al.ConfirmFlag) && (al.InsertDate >= date))
                .OrderByDescending(al => al.InsertDate).FirstOrDefault();

            return alarm;
        }
    }
}
