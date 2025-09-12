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
    public interface IClockRepository : IRepository<MClock>
    {
    }
    public class ClockRepository : RepositoryBase<MClock>, IClockRepository
    {
        public ClockRepository(IDbFactory dbFactory)
            : base(dbFactory)
        {
        }


        public override void Add(MClock clock)
        {
            if (String.IsNullOrEmpty(clock.InsertUser))
            {
                clock.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(clock.UpdateUser))
            {
                clock.UpdateUser = Constants.NULL;
            }

            clock.InsertDate = DateTime.Now;
            clock.UpdateDate = DateTime.Now;
            clock.VersionNo = Constants.VERSION_START;
            clock.DeleteFlg = Constants.FLAG_OFF;
            base.Add(clock);
        }

        public override void Update(MClock clock)
        {
            if (String.IsNullOrEmpty(clock.InsertUser))
            {
                clock.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(clock.UpdateUser))
            {
                clock.UpdateUser = Constants.NULL;
            }

            clock.UpdateDate = DateTime.Now;
            clock.VersionNo += 1;
            base.Update(clock);
        }
    }
}
