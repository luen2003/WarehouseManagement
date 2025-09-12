using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface ITankGrpTankRepository : IRepository<MTankGrpTank>
    {
    }
    public class TankGrpTankRepository : RepositoryBase<MTankGrpTank>, ITankGrpTankRepository
    {
        public TankGrpTankRepository(IDbFactory dbFactory)
            : base(dbFactory)
        {
        }


        public override void Add(MTankGrpTank tankgrptank)
        {
            base.Add(tankgrptank);
        }

        public override void Update(MTankGrpTank tankgrptank)
        {
            base.Update(tankgrptank);
        }
    }
}