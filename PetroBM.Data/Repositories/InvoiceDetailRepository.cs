using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface IInvoiceDetailRepository : IRepository<MInvoiceDetail>
    {
    }
    public class InvoiceDetailRepository : RepositoryBase<MInvoiceDetail>, IInvoiceDetailRepository
    {
        public InvoiceDetailRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }


        public override void Add(MInvoiceDetail invoicedetail)
        {
            if (String.IsNullOrEmpty(invoicedetail.InsertUser))
            {
                invoicedetail.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(invoicedetail.UpdateUser))
            {
                invoicedetail.UpdateUser = Constants.NULL;
            }

            invoicedetail.InsertDate = DateTime.Now;
            invoicedetail.UpdateDate = DateTime.Now;
            invoicedetail.VersionNo = Constants.VERSION_START;
            invoicedetail.DeleteFlg = Constants.FLAG_OFF;
            base.Add(invoicedetail);
        }

        public override void Update(MInvoiceDetail invoicedetail)
        {
            if (String.IsNullOrEmpty(invoicedetail.InsertUser))
            {
                invoicedetail.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(invoicedetail.UpdateUser))
            {
                invoicedetail.UpdateUser = Constants.NULL;
            }

            invoicedetail.UpdateDate = DateTime.Now;
            invoicedetail.VersionNo += 1;
            base.Update(invoicedetail);
        }
    }
}