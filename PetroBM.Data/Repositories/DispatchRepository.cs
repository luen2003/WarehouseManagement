
using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface IDispatchRepository : IRepository<MDispatch>
    {
    }
    public class DispatchRepository : RepositoryBase<MDispatch>, IDispatchRepository
    {
        public DispatchRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }


        public override void Add(MDispatch dispatch)
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

        public override void Update(MDispatch dispatch)
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
