using log4net;
using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace PetroBM.Services.Services
{
    public interface IDensityService
    {
        IEnumerable<MDensity> GetAllDensity();
        IEnumerable<MDensity> GetAllDensityOrderByName();
        bool CreateDensity(MDensity density);
        bool UpdateDensity(MDensity density);
        bool DeleteDensity(int id);
        MDensity FindDensityById(int id);
    }

    public class DensityService : IDensityService
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(DensityService));
        private readonly IDensityRepository densityRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public DensityService(IDensityRepository densityRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.densityRepository = densityRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateDensity(MDensity density)
        {
            var rs = false;
            try
            {
                log.Info("CreateDensity");
                using (TransactionScope ts = new TransactionScope())
                {
                    densityRepository.Add(density);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_DENSITY_CREATE, density.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool DeleteDensity(int id)
        {
            MDensity density = this.FindDensityById(id);
            var rs = false;
            try
            {
                log.Info("DeleteDensity" + id);
                using (TransactionScope ts = new TransactionScope())
                {
                    density.DeleteFlg = Constants.FLAG_ON;
                    densityRepository.Update(density);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_DENSITY_DELETE, density.UpdateUser);
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public IEnumerable<MDensity> GetAllDensity()
        {
            log.Info("GetAllDensity");
            return densityRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(p => p.InsertDate);
        }

        public IEnumerable<MDensity> GetAllDensityOrderByName()
        {
            log.Info("GetAllDensityOrderByName");
            return densityRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(p => p.WareHouseCode);
        }

        public bool UpdateDensity(MDensity density)
        {
            var rs = false;
            try
            {
                log.Info("UpdateDensity");
                using (TransactionScope ts = new TransactionScope())
                {
                    densityRepository.Update(density);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_DENSITY_UPDATE, density.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public MDensity FindDensityById(int id)
        {
            log.Info("FindDensityById " + id);
            return densityRepository.GetById(id);
        }
    }
}