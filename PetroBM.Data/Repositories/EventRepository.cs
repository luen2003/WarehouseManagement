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
    public interface IEventRepository : IRepository<MEvent>
    {
    }

    public class EventRepository : RepositoryBase<MEvent>, IEventRepository
    {
        public EventRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        public override void Add(MEvent newEvent)
        {
            newEvent.InsertDate = DateTime.Now;
            base.Add(newEvent);
        }        
    }
}
