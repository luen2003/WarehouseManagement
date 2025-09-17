using log4net;
using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        MImage GetImageByUsername(string username);

        bool CreateImage(MImage image);
        bool UpdateImage(MImage image);
        bool DeleteImage(int id);
    }

    public class ImageService : IImageService
    {
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
            log.Info("GetAllImage called");
            try
            {
                return imageRepository.GetAll();
            }
            catch (Exception ex)
            {
                log.Error("Error in GetAllImage", ex);
                return Enumerable.Empty<MImage>();
            }
        }

        public MImage FindImageById(int id)
        {
            log.Info($"FindImageById called with id={id}");
            try
            {
                return imageRepository.GetById(id);
            }
            catch (Exception ex)
            {
                log.Error($"Error in FindImageById with id={id}", ex);
                return null;
            }
        }

        public MImage GetImageByProcessStatus0()
        {
            log.Info("GetImageByProcessStatus0 called");
            try
            {
                return imageRepository.GetMany(x => x.ProcessStatus == 0).FirstOrDefault();
            }
            catch (Exception ex)
            {
                log.Error("Error in GetImageByProcessStatus0", ex);
                return null;
            }
        }

        public MImage GetImageByProcessStatus1()
        {
            log.Info("GetImageByProcessStatus1 called");
            try
            {
                return imageRepository.GetMany(x => x.ProcessStatus == 1).FirstOrDefault();
            }
            catch (Exception ex)
            {
                log.Error("Error in GetImageByProcessStatus1", ex);
                return null;
            }
        }

        public MImage GetImageByProcessStatus2()
        {
            log.Info("GetImageByProcessStatus2 called");
            try
            {
                return imageRepository.GetMany(x => x.ProcessStatus == 2).FirstOrDefault();
            }
            catch (Exception ex)
            {
                log.Error("Error in GetImageByProcessStatus2", ex);
                return null;
            }
        }
        public MImage GetImageByUsername(string username)
        {
            log.Info("GetImageByUsername called");
            try
            {
                return imageRepository.GetMany(x => x.ImageUser == username).FirstOrDefault();
            }
            catch (Exception ex)
            {
                log.Error("Error in GetImageByProcessStatus2", ex);
                return null;
            }
        }

        public bool CreateImage(MImage image)
        {
            log.Info("CreateImage called");
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    imageRepository.Add(image);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

            }
            catch (Exception ex)
            {
                log.Error("Error in CreateImage", ex);
            }
            return rs;
        }

        public bool UpdateImage(MImage image)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    imageRepository.Update(image);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

            }
            catch (Exception ex)
            {
                log.Error("Error in UpdateImage", ex);
            }
            return rs;
        }

        public bool DeleteImage(int id)
        {
            log.Info($"DeleteImage called for id={id}");
            var result = false;

            try
            {
                var image = imageRepository.GetById(id);
                if (image == null)
                {
                    log.Warn($"DeleteImage: Image with id={id} not found");
                    return false;
                }

                using (var ts = new TransactionScope())
                {
                    imageRepository.Delete(image);         // Xóa khỏi repository
                    unitOfWork.Commit();                   // Lưu vào DB
                    ts.Complete();                         // Hoàn tất transaction
                    result = true;
                    log.Info($"DeleteImage: Image id={id} deleted successfully");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error deleting image id={id}", ex);
            }

            return result;
        }

    }
}
