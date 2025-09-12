
using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface IConfigArmGrpConfigArmRepository : IRepository<MConfigArmGrpConfigArm>
    {
    }
    public class ConfigArmGrpConfigArmRepository : RepositoryBase<MConfigArmGrpConfigArm>, IConfigArmGrpConfigArmRepository
    {
        public ConfigArmGrpConfigArmRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }


        public override void Add(MConfigArmGrpConfigArm configarmgrpconfigarm)
        {
            base.Add(configarmgrpconfigarm);
        }

        public override void Update(MConfigArmGrpConfigArm configarmgrpconfigarm)
        {
            base.Update(configarmgrpconfigarm);
        }
    }
}