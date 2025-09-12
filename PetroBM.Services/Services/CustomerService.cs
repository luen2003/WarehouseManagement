using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.IO;
using log4net;

namespace PetroBM.Services.Services
{
    public interface ICustomerService
    {
        IEnumerable<MCustomer> GetAllCustomer();
        IEnumerable<MCustomer> GetAllCustomerOrderByName();
        IEnumerable<MCustomer> GetCustomerCode(string customercode);
        bool CreateCustomer(MCustomer customer);
        bool UpdateCustomer(MCustomer customer);
        bool DeleteCustomer(int id);
        MCustomer FindCustomerById(int id);
        bool Import(HttpPostedFileBase file, string user);

    }

    public class CustomerService : ICustomerService
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(CustomerService));

        private readonly ICustomerRepository customerRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public CustomerService(ICustomerRepository customerRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.customerRepository = customerRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateCustomer(MCustomer customer)
        {
            var rs = false;
            try
            {
                log.Info("CreateCustomer");
                using (TransactionScope ts = new TransactionScope())
                {
                    customerRepository.Add(customer);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_CUSTOMER_CREATE, customer.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool DeleteCustomer(int id)
        {
            MCustomer customer = this.FindCustomerById(id);
            var rs = false;
            try
            {
                log.Info("DeleteCustomer");
                using (TransactionScope ts = new TransactionScope())
                {
                    customer.DeleteFlg = Constants.FLAG_ON;
                    customerRepository.Update(customer);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_CUSTOMER_DELETE, customer.UpdateUser);
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public IEnumerable<MCustomer> GetAllCustomer()
        {
            log.Info("GetAllCustomer");
            return customerRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(p => p.InsertDate);
        }

        public IEnumerable<MCustomer> GetAllCustomerOrderByName()
        {
            log.Info("GetAllCustomerOrderByName");
            return customerRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(p => p.CustomerName);
        }
        public IEnumerable<MCustomer> GetCustomerCode(string customercode)
        {
            log.Info("GetCustomerCode" + customercode);
            return customerRepository.GetAll().Where(x => x.CustomerCode.ToLower() == customercode.ToLower());
        }

        public IEnumerable<MCustomer> GetCustomerCodeContain(string customercode)
        {
            log.Info("GetCustomerCode" + customercode);
            return customerRepository.GetAll().Where(x => x.CustomerCode.ToLower().Contains(customercode.ToLower()));
        }

        public bool UpdateCustomer(MCustomer customer)
        {
            var rs = false;
            try
            {
                log.Info("UpdateCustomer");
                using (TransactionScope ts = new TransactionScope())
                {
                    customerRepository.Update(customer);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_CUSTOMER_UPDATE, customer.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public MCustomer FindCustomerById(int id)
        {
            log.Info("FindCustomerById" + id);
            return customerRepository.GetById(id);
        }

        public bool Import(HttpPostedFileBase file, string user)
        {
            var rs = false;

            try
            {
                log.Info("Import" + file + user);
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    var path = Path.Combine(HttpContext.Current.Server.MapPath(Constants.FILE_PATH), fileName);
                    file.SaveAs(path);

                    var data = ExcelUtil.CommonReadFromExcelfile(path, Constants.Import_Column_Customer);
                    using (TransactionScope ts = new TransactionScope())
                    {
                        var lstCustomer = new List<MCustomer>();
                        foreach (System.Data.DataRow row in data.Rows)
                        {

                            var objectCustomer = new MCustomer();
                            objectCustomer.CustomerCode = row.ItemArray[0].ToString();
                            objectCustomer.CustomerName = row.ItemArray[1].ToString();
                            objectCustomer.TaxCode = row.ItemArray[2].ToString();
                            objectCustomer.AccountNo = row.ItemArray[3].ToString();
                            objectCustomer.PhoneNumber = row.ItemArray[4].ToString();
                            objectCustomer.Deputy = row.ItemArray[5].ToString();
                            objectCustomer.Position = row.ItemArray[6].ToString();
                            objectCustomer.CustomerAddress = row.ItemArray[7].ToString();
                            objectCustomer.UnitName = row.ItemArray[8].ToString();
                            objectCustomer.Payments = row.ItemArray[9].ToString();
                            objectCustomer.Price = row.ItemArray[10].ToString();
                            objectCustomer.Unit = row.ItemArray[11].ToString();                        
                            objectCustomer.InsertUser = user;
                            objectCustomer.UpdateUser = user;
                            objectCustomer.InsertDate = DateTime.Now;
                            objectCustomer.UpdateDate = DateTime.Now;
                            objectCustomer.VersionNo = Constants.VERSION_START;
                            objectCustomer.DeleteFlg = Constants.FLAG_OFF;
                            lstCustomer.Add(objectCustomer);
                        }

                        customerRepository.AddRange(lstCustomer);
                        unitOfWork.Commit();
                        ts.Complete();
                        rs = true;
                    }

                    // Log event
                    eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                        Constants.EVENT_CONFIG_CUSTOMER_IMPORT, user);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }


    }
}