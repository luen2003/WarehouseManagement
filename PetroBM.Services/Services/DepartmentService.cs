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
    public interface IDepartmentService
    {
        IEnumerable<MDepartment> GetAllDepartment();
        IEnumerable<MDepartment> GetAllDepartmentOrderByName();
        IEnumerable<MDepartment> GetDepartmentCode(string deparmentcode);
        bool CreateDepartment(MDepartment deparment);
        bool UpdateDepartment(MDepartment deparment);
        bool DeleteDepartment(int id);
        MDepartment FindDepartmentById(int id); 

    }

    public class DepartmentService : IDepartmentService
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(DepartmentService));

        private readonly IDepartmentRepository departmentRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public DepartmentService(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.departmentRepository = departmentRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateDepartment(MDepartment department)
        {
            var rs = false;
            try
            {
                log.Info("CreateDeparment");
                using (TransactionScope ts = new TransactionScope())
                {
                    departmentRepository.Add(department);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_CUSTOMER_CREATE, department.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool DeleteDepartment(int id)
        {
            MDepartment department = this.FindDepartmentById(id);
            var rs = false;
            try
            {
                log.Info("DeleteDeparment");
                using (TransactionScope ts = new TransactionScope())
                {
                    department.DeleteFlg = Constants.FLAG_ON;
                    departmentRepository.Update(department);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_CUSTOMER_DELETE, department.UpdateUser);
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public IEnumerable<MDepartment> GetAllDepartment()
        {
            log.Info("GetAllDeparment");
            return departmentRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(p => p.InsertDate);
        }

        public IEnumerable<MDepartment> GetAllDepartmentOrderByName()
        {
            log.Info("GetAllDeparmentOrderByName");
            return departmentRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(p => p.Name);
        }
        public IEnumerable<MDepartment> GetDepartmentCode(string departmentcode)
        {
            log.Info("Getdepartmentcode" + departmentcode);
            return departmentRepository.GetAll().Where(x => x.Code.ToLower() == departmentcode.ToLower());
        }

        public IEnumerable<MDepartment> GetDeparmentCodeContain(string departmentcode)
        {
            log.Info("GetDeparmentCode" + departmentcode);
            return departmentRepository.GetAll().Where(x => x.Code.ToLower().Contains(departmentcode.ToLower()));
        }

        public bool UpdateDepartment(MDepartment department)
        {
            var rs = false;
            try
            {
                log.Info("UpdateDeparment");
                using (TransactionScope ts = new TransactionScope())
                {
                    departmentRepository.Update(department);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_CUSTOMER_UPDATE, department.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public MDepartment FindDepartmentById(int id)
        {
            log.Info("FindDeparmentById" + id);
            return departmentRepository.GetById(id);
        }

        


    }
}