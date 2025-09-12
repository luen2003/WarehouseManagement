using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface IPriceRepository : IRepository<MPrice>
    {
    }
    public class PriceRepository : RepositoryBase<MPrice>, IPriceRepository
    {
        public PriceRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }


        public override void Add(MPrice price)
        {
            if (String.IsNullOrEmpty(price.InsertUser))
            {
                price.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(price.UpdateUser))
            {
                price.UpdateUser = Constants.NULL;
            }

            price.InsertDate = DateTime.Now;
            price.UpdateDate = DateTime.Now;
            price.VersionNo = Constants.VERSION_START;
            price.DeleteFlg = Constants.FLAG_OFF;
            base.Add(price);
        }

        public override void Update(MPrice price)
        {
            if (String.IsNullOrEmpty(price.InsertUser))
            {
                price.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(price.UpdateUser))
            {
                price.UpdateUser = Constants.NULL;
            }

            price.UpdateDate = DateTime.Now;
            price.VersionNo += 1;
            base.Update(price);
        }
    }
}