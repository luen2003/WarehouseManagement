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
    public interface IEventTypeRepository : IRepository<MEventType>
    {
    }

    public class EventTypeRepository : RepositoryBase<MEventType>, IEventTypeRepository
    {
        public EventTypeRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }
}
