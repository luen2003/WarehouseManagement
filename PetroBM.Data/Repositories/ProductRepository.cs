using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface IProductRepository : IRepository<MProduct>
    {
    }
    public class ProductRepository : RepositoryBase<MProduct>, IProductRepository
    {
        public ProductRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }


        public override void Add(MProduct product)
        {
            if (String.IsNullOrEmpty(product.InsertUser))
            {
                product.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(product.UpdateUser))
            {
                product.UpdateUser = Constants.NULL;
            }

            product.InsertDate = DateTime.Now;
            product.UpdateDate = DateTime.Now;
            product.VersionNo = Constants.VERSION_START;
            product.DeleteFlg = Constants.FLAG_OFF;
            base.Add(product);
        }

        public override void Update(MProduct product)
        {
            if (String.IsNullOrEmpty(product.InsertUser))
            {
                product.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(product.UpdateUser))
            {
                product.UpdateUser = Constants.NULL;
            }

            product.UpdateDate = DateTime.Now;
            product.VersionNo += 1;
            base.Update(product);
        }
    }
}
