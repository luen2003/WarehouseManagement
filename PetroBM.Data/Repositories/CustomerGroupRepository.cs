using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface ICustomerGroupRepository : IRepository<MCustomerGroup>
    {
    }
    public class CustomerGroupRepository : RepositoryBase<MCustomerGroup>, ICustomerGroupRepository
    {
        public CustomerGroupRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public override void Add(MCustomerGroup customerGroup)
        {
            if (String.IsNullOrEmpty(customerGroup.InsertUser))
            {
                customerGroup.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(customerGroup.UpdateUser))
            {
                customerGroup.UpdateUser = Constants.NULL;
            }

            customerGroup.InsertDate = DateTime.Now;
            customerGroup.UpdateDate = DateTime.Now;
            customerGroup.VersionNo = Constants.VERSION_START;
            customerGroup.DeleteFlg = Constants.FLAG_OFF;
            base.Add(customerGroup);
        }

        public override void Update(MCustomerGroup customerGroup)
        {
            if (String.IsNullOrEmpty(customerGroup.InsertUser))
            {
                customerGroup.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(customerGroup.UpdateUser))
            {
                customerGroup.UpdateUser = Constants.NULL;
            }

            customerGroup.UpdateDate = DateTime.Now;
            customerGroup.VersionNo += 1;
            base.Update(customerGroup);
        }
    }
}