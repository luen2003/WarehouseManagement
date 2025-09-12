using PetroBM.Common.Util;
using PetroBM.Data;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Transactions;

namespace PetroBM.Services.Services
{
    public interface IInvoiceDetailService
    {
        IEnumerable<MInvoiceDetail> GetAllInvoiceDetail();
        IEnumerable<MInvoiceDetail> GetAllInvoiceDetailNoOrder();
        IEnumerable<MInvoiceDetail> GetAllInvoiceDetailOrderByName();
        bool CreateInvoiceDetail(MInvoiceDetail invoicedetail);
        bool UpdateInvoiceDetail(MInvoiceDetail invoicedetail);
        bool DeleteInvoiceDetail(int id);
        bool UpdateNoteInvoiceDetailById(int id, string note,string updateUser);  
        bool DeleteInvoiceDetail_By_InvoiceID(int InvoiceID);

        MInvoiceDetail FindInvoiceDetailById(int id);
        MInvoiceDetail GetLastInvoiceDetail();

        bool UpdateFlagbyCommandid(int id);

    }

    public class InvoiceDetailService : IInvoiceDetailService
    {
        private readonly IInvoiceDetailRepository invoicedetailRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;
        private readonly ICommandDetailService commandDetailService;
        private readonly ICommandDetailRepository commandDetailRepository;

        public InvoiceDetailService(IInvoiceDetailRepository invoicedetailRepository, ICommandDetailRepository commandDetailRepository, IUnitOfWork unitOfWork, IEventService eventService, ICommandDetailService commandDetailService)
        {
            this.invoicedetailRepository = invoicedetailRepository;
            this.commandDetailRepository = commandDetailRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
            this.commandDetailService = commandDetailService;
        }

        public bool CreateInvoiceDetail(MInvoiceDetail invoicedetail)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    invoicedetailRepository.Add(invoicedetail);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                //eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                //    Constants.EVENT_CONFIG_PRODUCT, invoicedetail.InsertUser);
            }
            catch (Exception e) { }

            return rs;
        }

        public bool DeleteInvoiceDetail_By_InvoiceID(int InvoiceID)
        {
           // MInvoiceDetail invoicedetail = this.FindInvoiceDetailById(id);
            var rs = false;
            var lstdetail = GetAllInvoiceDetail().Where(x => x.InvoiceID == InvoiceID);

            try
            { 
                using (TransactionScope ts = new TransactionScope())
                {
                    foreach (var item in lstdetail)
                    {
                        item.DeleteFlg = Constants.FLAG_ON;
                        invoicedetailRepository.Update(item);
                        // Log event
                        eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                            Constants.EVENT_CONFIG_TANK_DELETE, item.UpdateUser);
                    }
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }
            }
            catch { }

            return rs;
        }

        public bool DeleteInvoiceDetail(int id)
        {
            MInvoiceDetail invoicedetail = this.FindInvoiceDetailById(id);
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    invoicedetail.DeleteFlg = Constants.FLAG_ON;
                    invoicedetailRepository.Update(invoicedetail);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_TANK_DELETE, invoicedetail.UpdateUser);
            }
            catch { }

            return rs;
        }


        public IEnumerable<MInvoiceDetail> GetAllInvoiceDetail()
        {
            return invoicedetailRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF).OrderByDescending(p=>p.ID);
            //    .OrderByDescending(p => p.InsertDate);
        }

        public IEnumerable<MInvoiceDetail> GetAllInvoiceDetailNoOrder()
        {
            return invoicedetailRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF);
        }

        public IEnumerable<MInvoiceDetail> GetAllInvoiceDetailOrderByName()
        {
            return invoicedetailRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(p => p.OutTime);
        }

        public bool UpdateInvoiceDetail(MInvoiceDetail invoicedetail)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    invoicedetailRepository.Update(invoicedetail);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_PRODUCT, invoicedetail.UpdateUser);
            }
            catch (Exception e) {  }

            return rs;
        }

        public MInvoiceDetail FindInvoiceDetailById(int id)
        {
            return invoicedetailRepository.GetById(id);
        }

        public MInvoiceDetail GetLastInvoiceDetail()
        {
            MInvoiceDetail invoiceDetails = invoicedetailRepository.GetAll().OrderByDescending(x => x.ID).FirstOrDefault();

            return invoiceDetails;
        }

        public bool UpdateNoteInvoiceDetailById(int id, string note,string updateUser)
        {
            var rs = false;
            var invoicedetail = FindInvoiceDetailById(id);

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    if(note.Length>0)
                    {
                        invoicedetail.Note = note;
                        invoicedetail.UpdateDate = DateTime.Now;
                        invoicedetail.UpdateUser = updateUser; 
                        invoicedetailRepository.Update(invoicedetail);
                        unitOfWork.Commit();
                        ts.Complete();
                        rs = true;
                    }
                    
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_PRODUCT, invoicedetail.UpdateUser);
            }
            catch (Exception e) { }


            return rs;
        }


        public bool UpdateFlagbyCommandid(int commandId)
        {
            var rs = false;
            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Update_Flag_CommandDetail_By_CommandID";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param1 = new SqlParameter("CommandID", commandId); 
                            cmd.Parameters.Add(param1); 
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_COMMAND_MOVE, "user");
            }
            catch (Exception ex)
            { 

            }

            return rs;
        }

    }
}