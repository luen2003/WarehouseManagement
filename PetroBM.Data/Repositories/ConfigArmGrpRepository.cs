using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface IConfigArmGrpRepository : IRepository<MConfigArmGrp>
    {
    }
    public class ConfigArmGrpRepository : RepositoryBase<MConfigArmGrp>, IConfigArmGrpRepository
    {
        public ConfigArmGrpRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }


        public override void Add(MConfigArmGrp configarmgrp)
        {
            if (String.IsNullOrEmpty(configarmgrp.InsertUser))
            {
                configarmgrp.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(configarmgrp.UpdateUser))
            {
                configarmgrp.UpdateUser = Constants.NULL;
            }

            configarmgrp.InsertDate = DateTime.Now;
            configarmgrp.UpdateDate = DateTime.Now;
            configarmgrp.VersionNo = Constants.VERSION_START;
            configarmgrp.DeleteFlg = Constants.FLAG_OFF;
            base.Add(configarmgrp);
        }

        public override void Update(MConfigArmGrp configarmgrp)
        {
            if (String.IsNullOrEmpty(configarmgrp.InsertUser))
            {
                configarmgrp.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(configarmgrp.UpdateUser))
            {
                configarmgrp.UpdateUser = Constants.NULL;
            }

            configarmgrp.UpdateDate = DateTime.Now;
            configarmgrp.VersionNo += 1;
            base.Update(configarmgrp);
        }
    }
}
