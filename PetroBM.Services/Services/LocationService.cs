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
    public interface ILocationService
    {
        IEnumerable<MLocation> GetAllLocation();
        // IEnumerable<MLocation> GetAllLocationOrderByName();
        bool CreateLocation(MLocation location);
         bool UpdateLocation(MLocation location);
        bool DeleteLocation(int id);
        // // bool Import(HttpPostedFileBase file, string user);
        MLocation FindLocationById(int id);
    }

    public class LocationService : ILocationService
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(LocationService));
        private readonly ILocationRepository locationRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public LocationService(ILocationRepository locationRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.locationRepository = locationRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public IEnumerable<MLocation> GetAllLocation()
        {
            log.Info("GetAllLocation");
            return locationRepository.GetAll().Where(l => l.DeleteFlg == Constants.FLAG_OFF).OrderByDescending(l => l.InsertDate);
        }

        public bool CreateLocation(MLocation location)
        {
            var rs = false;
            try
            {
                log.Info("Start CreateLocation");
                using (TransactionScope ts = new TransactionScope())
                {
                    locationRepository.Add(location);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_LOCATION_CREATE, location.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish CreateLocation");
            return rs;
        }

        public bool UpdateLocation(MLocation location)
        {
            var rs = false;
            try
            {
                log.Info("Start UpdateLocation");
                using (TransactionScope ts = new TransactionScope())
                {
                    locationRepository.Update(location);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_LOCATION_UPDATE, location.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish UpdateLocation");
            return rs;
        }

        public bool DeleteLocation(int id)
        {
            MLocation location = this.FindLocationById(id);

            var rs = false;
            log.Info("Start DeleteLocation");
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    location.DeleteFlg = Constants.FLAG_ON;
                    locationRepository.Update(location);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_LOCATION_DELETE, location.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish DeleteLocation");
            return rs;
        }

        public MLocation FindLocationById(int id)
        {
            return locationRepository.GetById(id);
        }
    }
}