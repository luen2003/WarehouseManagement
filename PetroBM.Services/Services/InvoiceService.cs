using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using log4net;

namespace PetroBM.Services.Services
{
    public interface IInvoiceService
    {
        IEnumerable<MInvoice> GetAllInvoice();
        IEnumerable<MInvoice> GetAllInvoiceOrderByName();
        bool CreateInvoice(MInvoice invoice);
        bool UpdateInvoice(MInvoice invoice);
        bool DeleteInvoice(int id);
        int GetNewId();
        MInvoice FindInvoiceById(int id);
        int GetID(decimal? workOrder);
    }

    public class InvoiceService : IInvoiceService
    {
        ILog log = log4net.LogManager.GetLogger(typeof(InvoiceService));
        private readonly IInvoiceRepository invoiceRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public InvoiceService(IInvoiceRepository invoiceRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.invoiceRepository = invoiceRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateInvoice(MInvoice invoice)
        {
            var rs = false;
            log.Info("Start CreateInvoice");
            
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    invoiceRepository.Add(invoice);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_INVOICE_CREATE, invoice.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish CreateInvoice");
            return rs;
        }

        public bool DeleteInvoice(int id)
        {
            MInvoice invoice = this.FindInvoiceById(id);
            var rs = false;
            log.Info("Start DeleteInvoice");
           
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    invoice.DeleteFlg = Constants.FLAG_ON;
                    invoiceRepository.Update(invoice);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_INVOICE_DELETE, invoice.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish DeleteInvoice");
            return rs;
        }

        public IEnumerable<MInvoice> GetAllInvoice()
        {
            return invoiceRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF).OrderByDescending(p => p.InvoiceID);
               // .OrderByDescending(p => p.InsertDate);
        }

        public IEnumerable<MInvoice> GetAllInvoiceOrderByName()
        {
            return invoiceRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(p => p.VersionNo);
        }

        public bool UpdateInvoice(MInvoice invoice)
        {
            var rs = false;
            log.Info("Start UpdateInvoice");
            
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    invoiceRepository.Update(invoice);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_INVOICE_UPDATE, invoice.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish UpdateInvoice");
            return rs;
        }

        public MInvoice FindInvoiceById(int id)
        {
            return invoiceRepository.GetById(id);
        }
        public int GetID (decimal? workOrder)
        {


            var list= invoiceRepository.GetAll().Where(x => x.WorkOrder==workOrder);
            return list.Max(x => x.InvoiceID);
        }
        public int GetNewId()
        {
            return invoiceRepository.GetAll().Max(x => x.InvoiceID);
        }
       
    }
}