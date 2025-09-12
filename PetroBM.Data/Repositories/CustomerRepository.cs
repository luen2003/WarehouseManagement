using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface ICustomerRepository : IRepository<MCustomer>
    {
    }
    public class CustomerRepository : RepositoryBase<MCustomer>, ICustomerRepository
    {
        public CustomerRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public override void Add(MCustomer customer)
        {
            if (String.IsNullOrEmpty(customer.InsertUser))
            {
                customer.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(customer.UpdateUser))
            {
                customer.UpdateUser = Constants.NULL;
            }

            customer.InsertDate = DateTime.Now;
            customer.UpdateDate = DateTime.Now;
            customer.VersionNo = Constants.VERSION_START;
            customer.DeleteFlg = Constants.FLAG_OFF;
            base.Add(customer);
        }

        public override void Update(MCustomer customer)
        {
            if (String.IsNullOrEmpty(customer.InsertUser))
            {
                customer.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(customer.UpdateUser))
            {
                customer.UpdateUser = Constants.NULL;
            }

            customer.UpdateDate = DateTime.Now;
            customer.VersionNo += 1;
            base.Update(customer);
        }
    }
}