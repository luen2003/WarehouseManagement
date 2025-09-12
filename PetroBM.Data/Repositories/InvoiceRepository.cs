using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface IInvoiceRepository : IRepository<MInvoice>
    {
    }
    public class InvoiceRepository : RepositoryBase<MInvoice>, IInvoiceRepository
    {
        public InvoiceRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public override void Add(MInvoice invoice)
        {
            if (String.IsNullOrEmpty(invoice.InsertUser))
            {
                invoice.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(invoice.UpdateUser))
            {
                invoice.UpdateUser = Constants.NULL;
            }

            invoice.InsertDate = DateTime.Now;
            invoice.UpdateDate = DateTime.Now;
            invoice.VersionNo = Constants.VERSION_START;
            invoice.DeleteFlg = Constants.FLAG_OFF;
            base.Add(invoice);
        }

        public override void Update(MInvoice invoice)
        {
            if (String.IsNullOrEmpty(invoice.InsertUser))
            {
                invoice.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(invoice.UpdateUser))
            {
                invoice.UpdateUser = Constants.NULL;
            }

            invoice.UpdateDate = DateTime.Now;
            invoice.VersionNo += 1;
            base.Update(invoice);
        }
    }
}
