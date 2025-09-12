
using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface IDispatchWaterRepository : IRepository<MDispatchWater>
    {
    }
    public class DispatchWaterRepository : RepositoryBase<MDispatchWater>, IDispatchWaterRepository
    {
        public DispatchWaterRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }


        public override void Add(MDispatchWater dispatch)
        {
            if (String.IsNullOrEmpty(dispatch.InsertUser))
            {
                dispatch.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(dispatch.UpdateUser))
            {
                dispatch.UpdateUser = Constants.NULL;
            }

            dispatch.InsertDate = DateTime.Now;
            dispatch.UpdateDate = DateTime.Now;
            dispatch.VersionNo = Constants.VERSION_START;
            dispatch.DeleteFlg = Constants.FLAG_OFF;
            base.Add(dispatch);
        }

        public override void Update(MDispatchWater dispatch)
        {
            if (String.IsNullOrEmpty(dispatch.InsertUser))
            {
                dispatch.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(dispatch.UpdateUser))
            {
                dispatch.UpdateUser = Constants.NULL;
            }

            dispatch.UpdateDate = DateTime.Now;
            dispatch.VersionNo += 1;
            base.Update(dispatch);
        }
    }
}




