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
    public interface ISealService
    {
        IEnumerable<MSeal> GetAllSeal();
        IEnumerable<MSeal> GetAllSealOrderByName();

        bool CreateSeal(MSeal seal);
        bool UpdateSeal(MSeal Seal);
        bool DeleteSeal(int id);

        MSeal FindSealById(int id);
    }

    public class SealService : ISealService
    {
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(SealService));

        private readonly ISealRepository sealRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public SealService(ISealRepository sealRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.sealRepository = sealRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }
        public bool CreateSeal(MSeal seal)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    sealRepository.Add(seal);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                //// Log event
                //eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                //    Constants.EVENT_CONFIG_SEAL_CREATE, seal.InsertUser);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return rs;
        }

        public bool DeleteSeal(int id)
        {
            MSeal seal = this.FindSealById(id);
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    seal.DeleteFlg = Constants.FLAG_ON;
                    sealRepository.Update(seal);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_SEAL_DELETE, seal.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public IEnumerable<MSeal> GetAllSeal()
        {
            return sealRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(p => p.InsertDate);
        }

        public IEnumerable<MSeal> GetAllSealOrderByName()
        {
            return sealRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(p => p.ID);
        }

        public bool UpdateSeal(MSeal seal)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    sealRepository.Update(seal);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_SEAL_UPDATE, seal.UpdateUser);
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            return rs;
        }

        public MSeal FindSealById(int id)
        {
            return sealRepository.GetById(id);
        }

       
    }
}