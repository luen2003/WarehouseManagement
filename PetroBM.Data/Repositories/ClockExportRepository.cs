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
    public interface IClockExportRepository : IRepository<MClockExport>
    {
    }
    public class ClockExportRepository : RepositoryBase<MClockExport>, IClockExportRepository
    {
        public ClockExportRepository(IDbFactory dbFactory)
            : base(dbFactory)
        {
        }


        public override void Add(MClockExport clockexport)
        {
            base.Add(clockexport);
        }

        public override void Update(MClockExport clockexport)
        {
            base.Update(clockexport);
        }
    }
}
