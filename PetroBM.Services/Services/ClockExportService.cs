using log4net;
using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace PetroBM.Services.Services
{
    public interface IClockExportService
    {
        IEnumerable<MClockExport> GetAllClockExport();
        bool CreateClockExport(MClockExport clockexport);
        bool UpdateClockExport(MClockExport clockexport);

        MClockExport FindClockExportById(int id);
    }

    public class ClockExportService : IClockExportService
    {
        ILog log = log4net.LogManager.GetLogger(typeof(ClockExportService));
        private readonly IClockExportRepository clockexportRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public ClockExportService(IClockExportRepository clockexportRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.clockexportRepository = clockexportRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateClockExport(MClockExport clockexport)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    clockexportRepository.Add(clockexport);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                //eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                //    Constants.EVENT_CONFIG_PRODUCT, "admin");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public IEnumerable<MClockExport> GetAllClockExport()
        {
            return clockexportRepository.GetAll();
        }


        public bool UpdateClockExport(MClockExport clockexport)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    clockexportRepository.Update(clockexport);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public MClockExport FindClockExportById(int id)
        {
            return clockexportRepository.GetById(id);
        }
    }
}
