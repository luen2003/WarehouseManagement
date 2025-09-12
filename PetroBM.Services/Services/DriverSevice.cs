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
    public interface IDriverService
    {
        IEnumerable<MDriver> GetAllDriver();
        IEnumerable<MDriver> GetAllDriverOrderByName();
        bool CreateDriver(MDriver driver);
        bool UpdateDriver(MDriver driver);
        bool DeleteDriver(int id);
        bool Import(HttpPostedFileBase file, string user);
        MDriver FindDriverById(int id);
    }

    public class DriverService : IDriverService
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(DriverService));
        private readonly IDriverRepository driverRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public DriverService(IDriverRepository driverRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.driverRepository = driverRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateDriver(MDriver driver)
        {
            var rs = false;
            try
            {
                log.Info("CreateDriver");
                using (TransactionScope ts = new TransactionScope())
                {
                    driverRepository.Add(driver);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_PRODUCT, driver.InsertUser);
            }
            catch (Exception ex) {
                log.Error(ex);
            }

            return rs;
        }

        public bool DeleteDriver(int id)
        {
            MDriver driver = this.FindDriverById(id);
            var rs = false;
            try
            {
                log.Info("DeleteDriver " + id);
                using (TransactionScope ts = new TransactionScope())
                {
                    driver.DeleteFlg = Constants.FLAG_ON;
                    driverRepository.Update(driver);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_DRIVER_DELETE, driver.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public IEnumerable<MDriver> GetAllDriver()
        {
            log.Info("GetAllDriver");
            return driverRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(p => p.InsertDate);
        }

        public IEnumerable<MDriver> GetAllDriverOrderByName()
        {
            log.Info("GetAllDriverOrderByName");
            return driverRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(p => p.Name);
        }

        public bool UpdateDriver(MDriver driver)
        {
            var rs = false;
            try
            {
                log.Info("UpdateDriver");
                using (TransactionScope ts = new TransactionScope())
                {
                    driverRepository.Update(driver);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_DRIVER_UPDATE, driver.UpdateUser);
            }
            catch (Exception ex) {

                log.Error(ex);
            }

            return rs;
        }

        public MDriver FindDriverById(int id)
        {
            log.Info("FindDriverById " + id);
            return driverRepository.GetById(id);
        }

        public bool Import(HttpPostedFileBase file, string user)
        {
            var rs = false;

            try
            {
                log.Info("Import " + Path.GetFileName(file.FileName) +  user);
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    var path = Path.Combine(HttpContext.Current.Server.MapPath(Constants.FILE_PATH), fileName);
                    file.SaveAs(path);

                    var data = ExcelUtil.CommonReadFromExcelfile(path, Constants.Import_Column_Driver);
                    using (TransactionScope ts = new TransactionScope())
                    {
                        var lstDriver = new List<MDriver>();
                        foreach (System.Data.DataRow row in data.Rows)
                        {
                            if (row.ItemArray[0] != null || row.ItemArray[0] != "")
                            {
                                var objectDriver = new MDriver();
                                objectDriver.ID = 0;
                                objectDriver.Name = row.ItemArray[0].ToString();
                                objectDriver.BirthDate = Convert.ToDateTime(row.ItemArray[1]);                                
                                objectDriver.IdentificationNumber = row.ItemArray[2].ToString();
                                objectDriver.DriversLicense = row.ItemArray[3].ToString();
                                objectDriver.DriversLicenseExpire = Convert.ToDateTime(row.ItemArray[4]);
                                objectDriver.SavetyCertificates = row.ItemArray[5].ToString();
                                objectDriver.SavetyCertificatesExpire = Convert.ToDateTime(row.ItemArray[6]);
                                objectDriver.PhoneNumber = row.ItemArray[7].ToString();
                                objectDriver.InsertUser = user;
                                objectDriver.UpdateUser = user;
                                objectDriver.InsertDate = DateTime.Now;
                                objectDriver.UpdateDate = DateTime.Now;
                                objectDriver.VersionNo = Constants.VERSION_START;
                                objectDriver.DeleteFlg = Constants.FLAG_OFF;
                                lstDriver.Add(objectDriver);
                            }
                            
                        }

                        driverRepository.AddRange(lstDriver);
                        unitOfWork.Commit();
                        ts.Complete();
                        rs = true;
                    }

                    // Log event
                    eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                        Constants.EVENT_CONFIG_DRIVER_IMPORT, user);
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