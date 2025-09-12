using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetroBM.Common.Util;
namespace PetroBM.Data.Repositories
{
    public interface IAlarmTypeRepository : IRepository<MAlarmType>
    {
    }

    public class AlarmTypeRepository : RepositoryBase<MAlarmType>, IAlarmTypeRepository
    {
        public AlarmTypeRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }
}
