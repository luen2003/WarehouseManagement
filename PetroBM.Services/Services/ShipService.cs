using log4net;
using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;

namespace PetroBM.Services.Services
{
    public interface IShipService
    {
        IEnumerable<MShip> GetAllShip();
        // IEnumerable<MShip> GetAllShipOrderByName();
        bool CreateShip(MShip ship);
        bool UpdateShip(MShip ship);
        bool DeleteShip(int id);
        // // bool Import(HttpPostedFileBase file, string user);
        MShip FindShipById(int id);
    }

    public class ShipService : IShipService
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(ShipService));
        private readonly IShipRepository shipRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public ShipService(IShipRepository shipRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.shipRepository = shipRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public IEnumerable<MShip> GetAllShip()
        {
            log.Info("GetAllShip");
            return shipRepository.GetAll().Where(s => s.DeleteFlg == Constants.FLAG_OFF).OrderByDescending(s => s.InsertDate);
        }

        public bool CreateShip(MShip ship)
        {
            var rs = false;
            try
            {
                log.Info("Start CreateShip");
                using (TransactionScope ts = new TransactionScope())
                {
                    shipRepository.Add(ship);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_SHIP_CREATE, ship.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return rs;
        }
        public bool UpdateShip(MShip ship)
        {
            var rs = false;
            try
            {
                log.Info("Start UpdateShip");
                using (TransactionScope ts = new TransactionScope())
                {
                    shipRepository.Update(ship);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_SHIP_UPDATE, ship.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                rs = false;
            }
            return rs;
        }

        public bool DeleteShip(int id)
        {
            MShip ship = FindShipById(id);

            var rs = false;
            try
            {
                log.Info("Start DeleteShip");
                using (TransactionScope ts = new TransactionScope())
                {
                    ship.DeleteFlg = Constants.FLAG_ON;
                    shipRepository.Update(ship);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_SHIP_DELETE, ship.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return rs;
        }

        public MShip FindShipById(int id)
        {
            return shipRepository.GetById(id);
        }
    }
}