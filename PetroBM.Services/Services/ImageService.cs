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
    public interface IImageService
    {
        IEnumerable<MImage> GetAllImage();
        MImage FindImageById(int id);
        MImage GetImageByProcessStatus0();
        MImage GetImageByProcessStatus1();
        MImage GetImageByProcessStatus2();
    }

    public class ImageService : IImageService
    {
        private static ILog Log { get; set; }
        private readonly IImageRepository imageRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        private readonly ILog log = LogManager.GetLogger(typeof(ImageService));

        public ImageService(IImageRepository imageRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.imageRepository = imageRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public IEnumerable<MImage> GetAllImage()
        {
            log.Info("GetAllImages");
            return imageRepository.GetAll();
                                  
        }

        public MImage FindImageById(int id)
        {
            return imageRepository.GetById(id);
        }

        public MImage GetImageByProcessStatus0()
        {
            log.Info("GetImageByProcessStatus0");
            return imageRepository
                   .GetMany(x => x.ProcessStatus == 0)
                   .FirstOrDefault();
        }

        public MImage GetImageByProcessStatus1()
        {
            log.Info("GetImageByProcessStatus1");
            return imageRepository
                   .GetMany(x => x.ProcessStatus == 1)
                   .FirstOrDefault();
        }

        public MImage GetImageByProcessStatus2()
        {
            log.Info("GetImageByProcessStatus2");
            return imageRepository
                   .GetMany(x => x.ProcessStatus == 2)
                   .FirstOrDefault();
        }
    }
}
