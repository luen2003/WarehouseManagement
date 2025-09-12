using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface IConfigArmRepository : IRepository<MConfigArm>
    {
    }
    public class ConfigArmRepository : RepositoryBase<MConfigArm>, IConfigArmRepository
    {
        public ConfigArmRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public override void Add(MConfigArm configarm)
        {
            if (String.IsNullOrEmpty(configarm.InsertUser))
            {
                configarm.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(configarm.UpdateUser))
            {
                configarm.UpdateUser = Constants.NULL;
            }

            configarm.InsertDate = DateTime.Now;
            configarm.UpdateDate = DateTime.Now;
            configarm.VersionNo = Constants.VERSION_START;
            configarm.DeleteFlg = Constants.FLAG_OFF;
            base.Add(configarm);
        }

        public override void Update(MConfigArm configarm)
        {
            if (String.IsNullOrEmpty(configarm.InsertUser))
            {
                configarm.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(configarm.UpdateUser))
            {
                configarm.UpdateUser = Constants.NULL;
            }

            configarm.UpdateDate = DateTime.Now;
            configarm.VersionNo += 1;
            base.Update(configarm);
        }
    }
}