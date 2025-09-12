using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface ILiveDataArmRepository : IRepository<MLiveDataArm>
    {
    }
    public class LiveDataArmRepository : RepositoryBase<MLiveDataArm>, ILiveDataArmRepository
    {
        public LiveDataArmRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }


        public override void Add(MLiveDataArm livedataarm)
        {
            //if (String.IsNullOrEmpty(livedataarm.InsertUser))
            //{
            //    livedataarm.InsertUser = Constants.NULL;
            //}

            //if (String.IsNullOrEmpty(livedataarm.UpdateUser))
            //{
            //    livedataarm.UpdateUser = Constants.NULL;
            //}

            //livedataarm.InsertDate = DateTime.Now;
            //livedataarm.UpdateDate = DateTime.Now;
            //livedataarm.VersionNo = Constants.VERSION_START;
            //livedataarm.DeleteFlg = Constants.FLAG_OFF;
            base.Add(livedataarm);
        }

        public override void Update(MLiveDataArm livedataarm)
        {
            //if (String.IsNullOrEmpty(livedataarm.InsertUser))
            //{
            //    livedataarm.InsertUser = Constants.NULL;
            //}

            //if (String.IsNullOrEmpty(livedataarm.UpdateUser))
            //{
            //    livedataarm.UpdateUser = Constants.NULL;
            //}

            //livedataarm.UpdateDate = DateTime.Now;
            //livedataarm.VersionNo += 1;
            base.Update(livedataarm);
        }
    }
}