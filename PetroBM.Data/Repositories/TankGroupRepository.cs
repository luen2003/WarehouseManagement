using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface ITankGroupRepository : IRepository<MTankGrp>
    {
    }
    public class TankGroupRepository : RepositoryBase<MTankGrp>, ITankGroupRepository
    {
        public TankGroupRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public override void Add(MTankGrp tankGrp)
        {
            if (String.IsNullOrEmpty(tankGrp.InsertUser))
            {
                tankGrp.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(tankGrp.UpdateUser))
            {
                tankGrp.UpdateUser = Constants.NULL;
            }

            tankGrp.InsertDate = DateTime.Now;
            tankGrp.UpdateDate = DateTime.Now;
            tankGrp.VersionNo = Constants.VERSION_START;
            tankGrp.DeleteFlg = Constants.FLAG_OFF;
            base.Add(tankGrp);
        }

        public override void Update(MTankGrp tankGrp)
        {
            if (String.IsNullOrEmpty(tankGrp.InsertUser))
            {
                tankGrp.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(tankGrp.UpdateUser))
            {
                tankGrp.UpdateUser = Constants.NULL;
            }

            tankGrp.UpdateDate = DateTime.Now;
            tankGrp.VersionNo += 1;
            base.Update(tankGrp);
        }

        public void DeleteChild(MTankGrp entity)
        {
            //DbContext.MTankGrps.AddRange();
            base.Update(entity);
        }
    }
}
