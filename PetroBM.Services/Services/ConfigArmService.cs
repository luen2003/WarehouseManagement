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
    public interface IConfigArmService
    {
        IEnumerable<MConfigArm> GetAllConfigArm();
        IEnumerable<MConfigArm> GetAllConfigArmOrderByName();
        IEnumerable<MConfigArm> GetAllConfigArmByActiveStatusAndArmName(int activestatus, string armname);
        IEnumerable<MConfigArm> GetAllConfigArmByActiveStatusAndArmName(byte wareHouseCode,int activestatus, string armname);
        IEnumerable<MConfigArm> GetConfigArmByArmNo(int armno);
        IEnumerable<MConfigArm> GetConfigArmByArmNo(byte wareHouseCode,byte armno);

        bool CreateConfigArm(MConfigArm ConfigArm);
        bool UpdateConfigArm(MConfigArm ConfigArm);
        bool DeleteConfigArm(int id);
        bool DeleteConfigArm(byte wareHouseCode, byte armno);

        bool Import(HttpPostedFileBase file, string user);
        MConfigArm FindConfigArmById(int id);
        MConfigArm FindConfigArmByArmNo(byte wareHouseCode, byte armno);

    }

    public class ConfigArmService : IConfigArmService
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(ConfigArmService));

        private readonly IConfigArmRepository configarmRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public ConfigArmService(IConfigArmRepository configarmRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {

            this.configarmRepository = configarmRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateConfigArm(MConfigArm ConfigArm)
        {
            var rs = false;
            try
            {
                log.Info("CreateConfigArm");
                using (TransactionScope ts = new TransactionScope())
                {
                    configarmRepository.Add(ConfigArm);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_PRODUCT, ConfigArm.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool DeleteConfigArm(int id)
        {
            MConfigArm ConfigArm = this.FindConfigArmById(id);
            var rs = false;
            try
            {
                log.Info("DeleteConfigArm");
                using (TransactionScope ts = new TransactionScope())
                {
                    ConfigArm.DeleteFlg = Constants.FLAG_ON;
                    configarmRepository.Delete(ConfigArm);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_CONFIGARM_DELETE, ConfigArm.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public bool DeleteConfigArm(byte wareHouseCode,byte armno)
        {
            MConfigArm ConfigArm = this.FindConfigArmByArmNo(wareHouseCode,armno);
            var rs = false;
            try
            {
                log.Info("DeleteConfigArm");
                using (TransactionScope ts = new TransactionScope())
                {
                    ConfigArm.DeleteFlg = Constants.FLAG_ON;
                    configarmRepository.Update(ConfigArm);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_CONFIGARM_DELETE, ConfigArm.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }


        public IEnumerable<MConfigArm> GetAllConfigArm()
        {
            log.Info("GetAllConfigArm");
            return configarmRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(p => p.InsertDate);
        }

        public IEnumerable<MConfigArm> GetAllConfigArmOrderByName()
        {
            log.Info("GetAllConfigArmOrderByName");
            return configarmRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(p => p.ArmNo);
        }

        public bool UpdateConfigArm(MConfigArm ConfigArm)
        {
            var rs = false;
            try
            {
                log.Info("UpdateConfigArm");
                using (TransactionScope ts = new TransactionScope())
                {
                    configarmRepository.Update(ConfigArm);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_CONFIGARM_UPDATE, ConfigArm.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return rs;
        }

        public MConfigArm FindConfigArmById(int id)
        {
            log.Info("FindConfigArmById " + id);
            return configarmRepository.GetById(id);
        }

        public MConfigArm FindConfigArmByArmNo(byte wareHouseCode,byte armno)
        {
            log.Info("FindConfigArmByArmno " + armno + " , WareHouseCode" + wareHouseCode);
            return configarmRepository.GetMany(x => x.ArmNo == armno && x.WareHouseCode == wareHouseCode).FirstOrDefault();
        }


        public bool Import(HttpPostedFileBase file, string user)
        {
            var rs = false;
            log.Info("Start Import ConfigArm");
            try
            {
                
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    var path = Path.Combine(HttpContext.Current.Server.MapPath(Constants.FILE_PATH), fileName);
                    file.SaveAs(path);

                    var data = ExcelUtil.CommonReadFromExcelfile(path, Constants.Import_Column_ConfigArm);
                    using (TransactionScope ts = new TransactionScope())
                    {
                        var lstCustomer = new List<MConfigArm>();
                        foreach (System.Data.DataRow row in data.Rows)
                        {

                            var objectConfigArm = new MConfigArm();

                            objectConfigArm.WareHouseCode = byte.Parse(row.ItemArray[0].ToString());
                            objectConfigArm.ArmName = row.ItemArray[1].ToString();
                            objectConfigArm.ArmNo = Convert.ToByte(row.ItemArray[2].ToString());
                            objectConfigArm.ProductCode_1 = row.ItemArray[3].ToString();
                            objectConfigArm.ProductCode_2 = row.ItemArray[4].ToString();
                            objectConfigArm.ProductCode_3 = row.ItemArray[5].ToString();
                            objectConfigArm.ActiveStatus = byte.Parse(row.ItemArray[6].ToString());
                            objectConfigArm.TypeExport = byte.Parse(row.ItemArray[7].ToString());

                            objectConfigArm.InsertUser = user;
                            objectConfigArm.UpdateUser = user;
                            objectConfigArm.InsertDate = DateTime.Now;
                            objectConfigArm.UpdateDate = DateTime.Now;
                            objectConfigArm.VersionNo = Constants.VERSION_START;
                            objectConfigArm.DeleteFlg = Constants.FLAG_OFF;
                            lstCustomer.Add(objectConfigArm);
                        }

                        configarmRepository.AddRange(lstCustomer);
                        unitOfWork.Commit();
                        ts.Complete();
                        rs = true;
                    }

                    // Log event
                    eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                        Constants.EVENT_CONFIG_CONFIGARM_IMPORT, user);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish Import ConfigArm");
            return rs;
        }

        public IEnumerable<MConfigArm> GetAllConfigArmByActiveStatusAndArmName(int activestatus, string armname)
        {
            log.Info("GetAllConfigArmByActiveStatusAndArmName " + activestatus + armname);
            return configarmRepository.GetAll().Where(x => x.ActiveStatus == activestatus && x.ArmName == armname);
        }

        public IEnumerable<MConfigArm> GetAllConfigArmByActiveStatusAndArmName(byte wareHouseCode,int activestatus, string armname)
        {
            log.Info("GetAllConfigArmByActiveStatusAndArmName " + activestatus + armname);
            return configarmRepository.GetAll().Where(x => x.ActiveStatus == activestatus && x.ArmName == armname && x.WareHouseCode== wareHouseCode);
        }

        public IEnumerable<MConfigArm> GetConfigArmByArmNo(int armno)
        {
            log.Info("GetAllConfigArmByActiveStatusAndArmName ");
            return configarmRepository.GetAll().Where(x=> x.ArmNo ==armno);
        }

        public IEnumerable<MConfigArm> GetConfigArmByArmNo(byte wareHouseCode, byte armno)
        {
            log.Info("GetAllConfigArmByActiveStatusAndArmName ");
            return configarmRepository.GetMany(x => x.WareHouseCode == wareHouseCode && x.ArmNo == armno);
        }
    }
}

