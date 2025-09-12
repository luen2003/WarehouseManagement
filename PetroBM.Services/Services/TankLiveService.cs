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
    public interface ITankLiveService
    {
        IEnumerable<MTankLive> GetTankLives_By_WareHouseCode(byte wareHouseCode);
        IEnumerable<MTankLive> GetTankLives_By_WareHouseCode_And_Tank_Id(byte wareHouseCode,int tankID);
        MTankLive GetTankLive_By_WareHouseCode_Tank_GetMaxTime(byte wareHouseCode, int tankId);
        bool CreateTankLive(MTankLive tanklive);
        bool UpdateTankLive(MTankLive tanklive);

    }

    public class TankLiveService : ITankLiveService
    {
        private readonly ITankLiveRepository tankliveRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public TankLiveService(ITankLiveRepository tankliveRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.tankliveRepository = tankliveRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateTankLive(MTankLive tanklive)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    tankliveRepository.Add(tanklive);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_PRODUCT, "admin");
            }
            catch (Exception e) { }

            return rs;
        }


        public bool UpdateTankLive(MTankLive tanklive)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    tankliveRepository.Update(tanklive);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_PRODUCT, "admin");
            }
            catch (Exception e) { }

            return rs;
        }

        public IEnumerable<MTankLive> GetTankLives_By_WareHouseCode(byte wareHouseCode)
        {
            return tankliveRepository.GetMany(x => x.WareHouseCode == wareHouseCode);
        }

        public MTankLive GetTankLive_By_WareHouseCode_Tank_GetMaxTime(byte wareHouseCode, int tankId)
        {
            return tankliveRepository.GetMany(x => x.WareHouseCode == wareHouseCode  && x.TankId == tankId).OrderByDescending(x => x.InsertDate).FirstOrDefault();
        }

        public IEnumerable<MTankLive> GetTankLives_By_WareHouseCode_And_Tank_Id(byte wareHouseCode, int tankID)
        {
            return tankliveRepository.GetMany(x => x.WareHouseCode == wareHouseCode && x.TankId == tankID); ;
        }

    }
}
