using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface ITankManualRepository : IRepository<MTankManual>
    {
    }
    public class TankManualRepository : RepositoryBase<MTankManual>, ITankManualRepository
    {
        public TankManualRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public override void Add(MTankManual tankManual)
        {
            if (String.IsNullOrEmpty(tankManual.InsertUser))
            {
                tankManual.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(tankManual.UpdateUser))
            {
                tankManual.UpdateUser = Constants.NULL;
            }
            
            tankManual.UpdateDate = DateTime.Now;
            tankManual.VersionNo = Constants.VERSION_START;
            tankManual.DeleteFlg = Constants.FLAG_OFF;
            base.Add(tankManual);
        }

        public override void Update(MTankManual tankManual)
        {
            if (String.IsNullOrEmpty(tankManual.InsertUser))
            {
                tankManual.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(tankManual.UpdateUser))
            {
                tankManual.UpdateUser = Constants.NULL;
            }

            tankManual.UpdateDate = DateTime.Now;
            tankManual.VersionNo += 1;
            base.Update(tankManual);
        }
    }
}
