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
    public interface ICustomerGroupService
    {
        IEnumerable<MCustomerGroup> GetAllCustomerGroup();
        IEnumerable<MCustomerGroup> GetAllCustomerGroupOrderByName();
        IEnumerable<MCustomerGroup> GetCustomerGroupCode(string customerGroupode);
        bool CreateCustomerGroup(MCustomerGroup customerGroup);
        bool UpdateCustomerGroup(MCustomerGroup customerGroup);
        bool DeleteCustomerGroup(int id);
        MCustomerGroup FindCustomerGroupById(int id);
        bool Import(HttpPostedFileBase file, string user);

    }

    public class CustomerGroupService : ICustomerGroupService
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(CustomerGroupService));

        private readonly ICustomerGroupRepository customerGroupRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public CustomerGroupService(ICustomerGroupRepository customerGroupRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.customerGroupRepository = customerGroupRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateCustomerGroup(MCustomerGroup customerGroup)
        {
            var rs = false;
            try
            {
                log.Info("CreateCustomerGroup");
                using (TransactionScope ts = new TransactionScope())
                {
                    customerGroupRepository.Add(customerGroup);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_CUSTOMER_CREATE, customerGroup.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool DeleteCustomerGroup(int id)
        {
            MCustomerGroup customerGroup = this.FindCustomerGroupById(id);
            var rs = false;
            try
            {
                log.Info("DeleteCustomerGroup");
                using (TransactionScope ts = new TransactionScope())
                {
                    customerGroup.DeleteFlg = Constants.FLAG_ON;
                    customerGroupRepository.Update(customerGroup);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_CUSTOMER_DELETE, customerGroup.UpdateUser);
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public IEnumerable<MCustomerGroup> GetAllCustomerGroup()
        {
            log.Info("GetAllCustomerGroup");
            return customerGroupRepository.GetAll();
        }

        public IEnumerable<MCustomerGroup> GetAllCustomerGroupOrderByName()
        {
            log.Info("GetAllCustomerOrderByName");
            return customerGroupRepository.GetAll().OrderBy(p => p.CustomerGroupName);
        }
        public IEnumerable<MCustomerGroup> GetCustomerGroupCode(string customercode)
        {
            log.Info("GetCustomerCode" + customercode);
            return customerGroupRepository.GetAll().Where(x => x.CustomerGroupCode.ToLower().Contains(customercode.ToLower()));
        }
        public bool UpdateCustomerGroup(MCustomerGroup customerGroup)
        {
            var rs = false;
            try
            {
                log.Info("UpdateCustomerGroup");
                using (TransactionScope ts = new TransactionScope())
                {
                    customerGroupRepository.Update(customerGroup);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_CUSTOMER_UPDATE, customerGroup.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public MCustomerGroup FindCustomerGroupById(int id)
        {
            log.Info("FindCustomerGroupById" + id);
            return customerGroupRepository.GetById(id);
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
                        var lstCustomer = new List<MCustomerGroup>();
                        foreach (System.Data.DataRow row in data.Rows)
                        {

                            var objectCustomerGroup = new MCustomerGroup();
                            objectCustomerGroup.CustomerGroupCode = row.ItemArray[0].ToString();
                            objectCustomerGroup.CustomerGroupName = row.ItemArray[1].ToString();
                            
                            lstCustomer.Add(objectCustomerGroup);
                        }

                        customerGroupRepository.AddRange(lstCustomer);
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