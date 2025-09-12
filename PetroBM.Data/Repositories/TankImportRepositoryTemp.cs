
using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface ITankImportTempRepository : IRepository<MTankImportTemp>
    {
    }
    public class TankImportTempRepository : RepositoryBase<MTankImportTemp>, ITankImportTempRepository
    {
        public TankImportTempRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

    }
}
