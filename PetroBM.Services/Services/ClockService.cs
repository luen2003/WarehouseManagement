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
    public interface IClockService
    {
        IEnumerable<MClock> GetAllClock();
        IEnumerable<MClock> GetAllClockOrderByName();
        bool CreateClock(MClock clock);
        bool UpdateClock(MClock clock);
        bool DeleteClock(int id);

        MClock FindClockById(int id);
    }

    public class ClockService : IClockService
    {
        ILog log = log4net.LogManager.GetLogger(typeof(ClockService));
        private readonly IClockRepository clockRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public ClockService(IClockRepository clockRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.clockRepository = clockRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateClock(MClock clock)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    clockRepository.Add(clock);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_CLOCK_CREATE, clock.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool DeleteClock(int id)
        {
            MClock clock = this.FindClockById(id);
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    clock.DeleteFlg = Constants.FLAG_ON;
                    clockRepository.Update(clock);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_CLOCK_DELETE, clock.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public IEnumerable<MClock> GetAllClock()
        {
            return clockRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(p => p.InsertDate);
        }

        public IEnumerable<MClock> GetAllClockOrderByName()
        {
            return clockRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(p => p.ClockName);
        }

        public bool UpdateClock(MClock clock)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    clockRepository.Update(clock);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_CLOCK_UPDATE, clock.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public MClock FindClockById(int id)
        {
            return clockRepository.GetById(id);
        }
    }
}
