using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using log4net;

namespace PetroBM.Services.Services
{
    public interface IPriceService
    {
        IEnumerable<MPrice> GetAllPrice();
        IEnumerable<MPrice> GetAllPriceOrderByName();
        bool CreatePrice(MPrice price);
        bool UpdatePrice(MPrice price);
        bool DeletePrice(int id);
        bool Import(HttpPostedFileBase file, string user);
        MPrice FindPriceById(int id);
    }

    public class PriceService : IPriceService
    {
        ILog log = log4net.LogManager.GetLogger(typeof(PriceService));

        private readonly IPriceRepository priceRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public PriceService(IPriceRepository priceRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.priceRepository = priceRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreatePrice(MPrice price)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    priceRepository.Add(price);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_PRICE_CREATE, price.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool DeletePrice(int id)
        {
            MPrice price = this.FindPriceById(id);
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    price.DeleteFlg = Constants.FLAG_ON;
                    priceRepository.Update(price);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_PRICE_DELETE, price.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public IEnumerable<MPrice> GetAllPrice()
        {
            return priceRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(p => p.InsertDate);
        }

        public IEnumerable<MPrice> GetAllPriceOrderByName()
        {
            return priceRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(p => p.ProductName);
        }

        public bool UpdatePrice(MPrice price)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    priceRepository.Update(price);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_PRICE_UPDATE, price.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public MPrice FindPriceById(int id)
        {
            return priceRepository.GetById(id);
        }
        public bool Import(HttpPostedFileBase file, string user)
        {
            var rs = false;

            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    var path = Path.Combine(HttpContext.Current.Server.MapPath(Constants.FILE_PATH), fileName);
                    file.SaveAs(path);

                    var data = ExcelUtil.CommonReadFromExcelfile(path, Constants.Import_Column_Price);
                    using (TransactionScope ts = new TransactionScope())
                    {
                        var lstPrice = new List<MPrice>();
                        foreach (System.Data.DataRow row in data.Rows)
                        {

                            var objectPrice = new MPrice();
                            objectPrice.ProductName = row.ItemArray[0].ToString();
                            objectPrice.Abbreviations = row.ItemArray[1].ToString();
                            objectPrice.ProductCode = row.ItemArray[2].ToString();
                            objectPrice.Unit = row.ItemArray[3].ToString();
                            objectPrice.AreaPrice1 = Convert.ToInt32(row.ItemArray[4]);
                            objectPrice.AreaPrice2 = Convert.ToInt32(row.ItemArray[5]);
                            objectPrice.InsertUser = user;
                            objectPrice.UpdateUser = user;
                            objectPrice.InsertDate = DateTime.Now;
                            objectPrice.UpdateDate = DateTime.Now;
                            objectPrice.VersionNo = Constants.VERSION_START;
                            objectPrice.DeleteFlg = Constants.FLAG_OFF;
                            objectPrice.EnvironmentTax = Convert.ToInt32(row.ItemArray[6]);
                            lstPrice.Add(objectPrice);
                        }

                        priceRepository.AddRange(lstPrice);
                        unitOfWork.Commit();
                        ts.Complete();
                        rs = true;
                    }

                    // Log event
                    eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                        Constants.EVENT_CONFIG_PRICE_IMPORT, user);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }
    }
}