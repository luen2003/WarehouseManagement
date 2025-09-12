using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface ITankImportRepository : IRepository<MTankImport>
    {
    }
    public class TankImportRepository : RepositoryBase<MTankImport>, ITankImportRepository
    {
        public TankImportRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public override void Add(MTankImport tankimport)
        {
            base.Add(tankimport);
        }

        public override void Update(MTankImport tankimport)
        {
            base.Update(tankimport);
        }
    }
}
