using PetroBM.Common.Util;
using PetroBM.Data;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PetroBM.Services.Services
{
    public interface ITankGrpTankService
    {
        IEnumerable<MTankGrpTank> GetAllTankGrpTank();
        IEnumerable<MTankGrpTank> GetAllTankGrpTankOrderByName();
        IEnumerable<MTankGrpTank> GetAllTankGrpTankByGrpId(int tankGrpTank);
        bool CreateTankGrpTank(MTankGrpTank tankgrptank);
        bool UpdateTankGrpTank(MTankGrpTank tankgrptank);
        bool DeleteTankGrpTank(int id);

        MTankGrpTank FindTankGrpTankById(int id);
    }

    public class TankGrpTankService : ITankGrpTankService
    {
        private readonly ITankGrpTankRepository tankgrptankRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public TankGrpTankService(ITankGrpTankRepository tankgrptankRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.tankgrptankRepository = tankgrptankRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateTankGrpTank(MTankGrpTank tankgrptank)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    tankgrptankRepository.Add(tankgrptank);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_PRODUCT, "Admin");
            }
            catch (Exception e) { }

            return rs;
        }

        public bool DeleteTankGrpTank(int id)
        {
            MTankGrpTank tankgrptank = this.FindTankGrpTankById(id);
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    tankgrptankRepository.Update(tankgrptank);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_TANK_DELETE, "admin");
            }
            catch { }

            return rs;
        }

        public IEnumerable<MTankGrpTank> GetAllTankGrpTank()
        {
            return tankgrptankRepository.GetAll();
        }

        public IEnumerable<MTankGrpTank> GetAllTankGrpTankByGrpId(int tankGrpTank)
        {
            return tankgrptankRepository.GetAll().Where(x => x.TankGrpId == tankGrpTank).OrderBy(x => x.TankId);
        }

        public IEnumerable<MTankGrpTank> GetAllTankGrpTankOrderByName()
        {
            return tankgrptankRepository.GetAll();
        }

        public bool UpdateTankGrpTank(MTankGrpTank tankgrptank)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    tankgrptankRepository.Update(tankgrptank);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_PRODUCT,"admin");
            }
            catch (Exception e) { }

            return rs;
        }

        public MTankGrpTank FindTankGrpTankById(int id)
        {
            return tankgrptankRepository.GetById(id);
        }
    }
}