using PagedList;
using PetroBM.Web.Attribute;
using PetroBM.Common.Util;
using PetroBM.Web.Models;
using PetroBM.Services.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace PetroBM.Web.Controllers
{
    [HasPermission(Constants.PERMISSION_IMPORT)]
    public class ImportController : BaseController
    {
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(ImportController));

        private ImportModel importModel;
        private readonly IImportService importService;
        private readonly IProductService productService;
        private readonly ITankService tankService;
        private readonly ITankGrpTankService tankGrpTankService;
        private readonly IWareHouseService wareHouseService;
        private readonly ITankImportService tankImportService;
        private readonly IClockService clockService;
        private readonly IClockExportService clockExportService;
        private readonly ITankImportTempService tankImportTempService;
        private readonly IExportArmImportService exportArmImportService;
        private readonly IConfigArmService configArmService;
        private readonly IReportService reportService;
        private readonly BaseService baseService;


        public ImportController(ImportModel importModel, IImportService importService,
            ITankGrpTankService tankGrpTankService, ITankImportService tankImportService, IClockService clockService, IClockExportService clockExportService,
            ITankImportTempService tankImportTempService, IProductService productService, IConfigArmService configArmService, IExportArmImportService exportArmImportService,
        ITankService tankService, IWareHouseService wareHouseService, IReportService reportService, BaseService baseService)
        {
            this.importModel = importModel;
            this.importService = importService;
            this.productService = productService;
            this.tankService = tankService;
            this.wareHouseService = wareHouseService;
            this.tankGrpTankService = tankGrpTankService;
            this.tankImportService = tankImportService;
            this.tankImportTempService = tankImportTempService;
            this.clockService = clockService;
            this.clockExportService = clockExportService;
            this.configArmService = configArmService;
            this.exportArmImportService = exportArmImportService;
            this.reportService = reportService;
            this.baseService = baseService;

        }

        // GET: Import
        public ActionResult Index(ImportModel importModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_IMPORTPRODUCT);
            if (checkPermission)
            {
                int pageNumber = (page ?? 1);

                DateTime? startDate = null;
                DateTime? endDate = null;

                if (!String.IsNullOrEmpty(importModel.StartDate))
                {
                    startDate = DateTime.ParseExact(importModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!String.IsNullOrEmpty(importModel.EndDate))
                {
                    endDate = DateTime.ParseExact(importModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                byte wareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());

                importModel.ImportInfoList = importService.GetImportInfoByTime(wareHouseCode, startDate, endDate)
                    .ToPagedList(pageNumber, Constants.PAGE_SIZE);
                importModel.TankImports = tankImportService.GetListTankImportByWareHouseCode(wareHouseCode).ToList();

                return View(importModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public ActionResult New(int productId = 0)
        {


            var wareHouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());
            importModel.ProductList = productService.GetAllProduct();
            importModel.WareHouseList = wareHouseService.GetAllWareHouse();
            importModel.ProductId = productId;
            // importModel.ListConfigArm = configArmService.GetAllConfigArm().Where(x => x.WareHouseCode == wareHouse).ToList();

            //var lstgrp = tankGrpTankService.GetAllTankGrpTank().Where(x => x.WareHouseCode == wareHouse).ToList();
            //List<int> lstTank = new List<int>();

            //for (int i = 0; i < lstgrp.Count; i++)
            //{
            //    lstTank.Add(lstgrp[i].TankId);
            //}

            if (productId == 0)
            {
                importModel.TankList = tankService.GetAllTank().Where(x => x.WareHouseCode == wareHouse);
            }
            else
            {
                importModel.TankList = tankService.GetAllTank().Where(x => x.WareHouseCode == wareHouse && x.ProductId == productId).OrderBy(x => x.TankName);
            }

            importModel.TankTemps = new List<TankTempModel>();
            foreach (var item in importModel.TankList)
            {
                var it = new TankTempModel();
                it.TankId = item.TankId;
                it.TankName = item.TankName;
                it.ProductId = (int)item.ProductId;
                importModel.TankTemps.Add(it);
            }

            return View(importModel);
        }

        [HttpPost]
        public ActionResult DoCreate(ImportModel importModel)
        {
            importModel.ImportInfo.InsertUser = HttpContext.User.Identity.Name;
            importModel.ImportInfo.ProductId = importModel.ProductId;
            importModel.ImportInfo.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
            if (!String.IsNullOrEmpty(importModel.CertificateTime))
            {
                importModel.ImportInfo.CertificateTime = DateTime.ParseExact(importModel.CertificateTime, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            var rs = importService.CreateImportInfo(importModel.ImportInfo, importModel.ListTankId, importModel.ListArmNo);

            if (rs == true)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_FAILED;
            }

            return RedirectToAction("Edit", new { id = importModel.ImportInfo.Id });
        }

        public ActionResult Edit(int id)
        {
            importModel.ImportInfo = importService.GetImportInfo(id);

            if (importModel.ImportInfo.CertificateTime != null)
            {
                importModel.CertificateTime = importModel.ImportInfo.CertificateTime.Value.ToString(Constants.DATE_FORMAT);
            }
            //var selected = importModel.ImportInfo.MTankImport.Where(tm => tm.TankId != Constants.TEC_CC_ID)
            //    .Select(tm => tm.MTank.TankName).ToArray();
            //string nameList = String.Empty;

            //foreach (var name in selected)
            //{
            //    nameList += name + ", ";
            //}

            //importModel.ListTankName = nameList.Remove(nameList.Length - 2, 2);
            var lstTankIdTankImport = tankImportService.GetTankImportByImportinfoId(byte.Parse(Session[Constants.Session_WareHouse].ToString()), id);
            List<int> lstTankId = new List<int>();

            foreach (var item in lstTankIdTankImport)
            {
                lstTankId.Add(item.TankId);
            }
            var listTankName = tankService.GetAllTank().Where(x => x.WareHouseCode == importModel.ImportInfo.WareHouseCode && lstTankId.Contains(x.TankId)).Select(x => x.TankName).ToArray();

            string nameList = String.Empty;
            foreach (var name in listTankName)
            {
                nameList += name + ", ";
            }
            importModel.ListTankName = nameList.Remove(nameList.Length - 2, 2);

            importModel.ListArmNoName = importService.GetListArmName(id);



            return View(importModel);
        }

        [HttpPost]
        public ActionResult DoEdit(ImportModel importModel)
        {
            var importInfo = importService.GetImportInfo(importModel.ImportInfo.Id);

            if (importInfo != null)
            {
                importInfo.Vehicle = importModel.ImportInfo.Vehicle;
                importInfo.Export = importModel.ImportInfo.Export;
                importInfo.Vtt = importModel.ImportInfo.Vtt;
                importInfo.V15 = importModel.ImportInfo.V15;
                importInfo.Temperature = importModel.ImportInfo.Temperature;
                importInfo.Density = importModel.ImportInfo.Density;
                importInfo.VCF = importModel.ImportInfo.VCF;
                importInfo.InputWastageRate = importModel.ImportInfo.InputWastageRate;
                importInfo.VendorName = importModel.ImportInfo.VendorName;
                importInfo.CertificateNumber = importModel.ImportInfo.CertificateNumber;
                if (!String.IsNullOrEmpty(importModel.CertificateTime))
                {
                    importInfo.CertificateTime = DateTime.ParseExact(importModel.CertificateTime, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                importInfo.UpdateUser = HttpContext.User.Identity.Name;

                var rs = importService.UpdateImportInfo(importInfo);

                if (rs)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                }
                else
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
                }
            }

            return RedirectToAction("Index");
        }


        public ActionResult Delete(int id)
        {
            var importInfo = importService.GetImportInfo(id);

            if (importInfo != null)
            {
                importInfo.UpdateUser = HttpContext.User.Identity.Name;
                var rs = importService.DeleteImportInfo(importInfo);

                if (rs)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_SUCCESS;
                }
                else
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_FAILED;
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult StartHandle(int id, bool? manualInput, bool? isIndex, string handleDate)
        {
            importModel.ImportInfo = importService.GetImportInfo(id);
            importModel.TankImports = tankImportService.GetTankImportByImportinfoId(byte.Parse(Session[Constants.Session_WareHouse].ToString()), id).ToList();
            importModel.TankImportTemps = tankImportTempService.GetTankImportTempByImportinfoId(byte.Parse(Session[Constants.Session_WareHouse].ToString()), id).ToList();
            importModel.ListExportArmImport = exportArmImportService.GetAllExportArmImportByImportInfoId(id).ToList();
            importModel.ListConfigArm = configArmService.GetAllConfigArm().Where(x => x.WareHouseCode == byte.Parse(Session[Constants.Session_WareHouse].ToString())).ToList();
            importModel.HandleDate = handleDate;
            if (importModel.HandleDate == null)
            {
                importModel.HandleDate = DateTime.Now.ToString(Constants.DATE_FORMAT); ;
            }

            //Lấy danh sách có trong bảng TankImport
            List<int> lstTank = new List<int>();
            foreach (var item in importModel.TankImports)
            {
                lstTank.Add(item.TankId);
            }

            importModel.TankList = tankService.GetAllTank().Where(x => x.WareHouseCode == importModel.ImportInfo.WareHouseCode && lstTank.ToArray().Contains(x.TankId));

            if (null != manualInput && manualInput == true)
            {
                importModel.ManualInput = true;
            }

            if (null == isIndex || isIndex == false)
            {
                var startDate = DateTime.ParseExact(importModel.HandleDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);

                for (int i = 0; i < importModel.TankImports.Count; i++)
                {
                    importModel.TankImports[i].StartDate = startDate;

                    //temp value
                    importModel.TankImportTemps[i].StartDate = startDate;

                    tankImportService.UpdateTankImport(importModel.TankImports[i]);
                    tankImportTempService.UpdateTankImportTemp(importModel.TankImportTemps[i]);
                }

                importModel.ImportInfo.MTankImport = importModel.TankImports;
                importModel.ImportInfo.MTankImportTemps = importModel.TankImportTemps;
                importService.GetStartHandleImportInfo(importModel.ImportInfo);
            }

            return View(importModel);
        }

        [HttpPost]
        public ActionResult GetStartHandleInfo(ImportModel importModel)
        {
            var importInfo = importService.GetImportInfo(importModel.ImportInfo.Id);

            if (importInfo != null)
            {
                var startDate = DateTime.ParseExact(importModel.HandleDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);

                for (int i = 0; i < importInfo.MTankImport.Count; i++)
                {
                    importInfo.MTankImport[i].StartDate = startDate;

                    //temp value
                    importInfo.MTankImportTemps[i].StartDate = startDate;
                }

                importService.GetStartHandleImportInfo(importInfo, startDate); // Update lại bảng từ TankImport từ Bảng TankLog sang
            }

            return RedirectToAction("StartHandle", new { id = importModel.ImportInfo.Id, isIndex = true, handleDate = importModel.HandleDate });
        }

        [HttpPost]
        public ActionResult DoStartHandle(ImportModel importModel)
        {
            var importInfo = importService.GetImportInfo(importModel.ImportInfo.Id);

            var lstTankImport = tankImportService.GetTankImportByImportinfoId(byte.Parse(Session[Constants.Session_WareHouse].ToString()), importModel.ImportInfo.Id).ToList();

            if (importInfo != null)
            {
                for (int i = 0; i < lstTankImport.Count(); i++)
                {
                    lstTankImport[i].StartDate = DateTime.ParseExact(importModel.HandleDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                    tankImportService.UpdateTankImport(lstTankImport[i]);
                }
                if (Request.Form["submitUpdate"] != null)
                {
                    importModel.StartDate = importModel.HandleDate;
                    importService.UpdateStartHandleImportInfo(importInfo);

                    for (int i = 0; i < lstTankImport.Count; i++)
                    {
                        var tankImport = importModel.TankImports[i];
                        lstTankImport[i].StartTemperature = tankImport.StartTemperature ?? 0;
                        lstTankImport[i].StartProductLevel = tankImport.StartProductLevel ?? 0;
                        lstTankImport[i].StartDensity = tankImport.StartDensity ?? 0;
                        tankImportService.UpdateTankImport(lstTankImport[i]);
                    }

                    //Update vào bảng  MExportArmImport Chỉ số đầu của họng
                    if (importModel.ListExportArmImport != null)
                    {
                        for (int i = 0; i < importModel.ListExportArmImport.Count; i++)
                        {
                            var ob = importModel.ListExportArmImport[i];
                            ob.ImportInfoId = importModel.ImportInfo.Id;
                            ob.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                            exportArmImportService.Update_StartExportArmImport(ob);
                        }
                    }


                    return RedirectToAction("StartHandle", new { id = importModel.ImportInfo.Id, manualInput = true, isIndex = true, handleDate = importModel.HandleDate });
                }
                else if (Request.Form["submitFinish"] != null)
                {
                    var rs = false;

                    if (importModel.ManualInput == true)
                    {
                        var message = importModel.ManualMessage;

                        rs = importService.FinishStartHandleImportInfoManual(importInfo, HttpContext.User.Identity.Name, message);

                        for (int i = 0; i < lstTankImport.Count; i++)
                        {
                            var tankImport = importModel.TankImports[i];
                            lstTankImport[i].StartTemperature = tankImport.StartTemperature ?? 0;
                            lstTankImport[i].StartProductLevel = tankImport.StartProductLevel ?? 0;
                            lstTankImport[i].StartDensity = tankImport.StartDensity ?? 0;
                            tankImportService.UpdateTankImport(lstTankImport[i]);
                        }

                        //Update vào bảng  MExportArmImport Chỉ số đầu của họng
                        if (importModel.ListExportArmImport != null)
                        {
                            for (int i = 0; i < importModel.ListExportArmImport.Count; i++)
                            {
                                var ob = importModel.ListExportArmImport[i];
                                ob.ImportInfoId = importModel.ImportInfo.Id;
                                ob.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                                exportArmImportService.Update_StartExportArmImport(ob);
                            }
                        }

                    }
                    else
                    {
                        rs = importService.FinishStartHandleImportInfo(importInfo, HttpContext.User.Identity.Name);
                        if (importModel.ListExportArmImport != null)
                        {
                            for (int i = 0; i < importModel.ListExportArmImport.Count; i++)
                            {
                                var ob = importModel.ListExportArmImport[i];
                                ob.ImportInfoId = importModel.ImportInfo.Id;
                                ob.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                                exportArmImportService.Update_StartExportArmImport(ob);
                            }
                        }
                    }

                    if (rs)
                    {
                        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                    }
                    else
                    {
                        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
                    }
                }

            }

            return RedirectToAction("StartHandle", new { id = importModel.ImportInfo.Id, isIndex = true, handleDate = importModel.HandleDate });
        }

        public ActionResult EndHandle(int id, bool? manualInput, bool? isIndex, string handleDate)
        {
            importModel.ImportInfo = importService.GetImportInfo(id);
            importModel.TankImports = tankImportService.GetTankImportByImportinfoId(byte.Parse(Session[Constants.Session_WareHouse].ToString()), id).ToList();
            importModel.TankImportTemps = tankImportTempService.GetTankImportTempByImportinfoId(byte.Parse(Session[Constants.Session_WareHouse].ToString()), id).ToList();
            importModel.ListConfigArm = configArmService.GetAllConfigArm().Where(x => x.WareHouseCode == byte.Parse(Session[Constants.Session_WareHouse].ToString())).ToList();
            importModel.ListExportArmImport = exportArmImportService.GetAllExportArmImportByImportInfoId(id).ToList();            
            //Lấy danh sách có trong bảng TankImport
            List<int> lstTank = new List<int>();
            foreach (var item in importModel.TankImports)
            {
                lstTank.Add(item.TankId);
            }

            importModel.HandleDate = handleDate;

            importModel.TankList = tankService.GetAllTank().Where(x => x.WareHouseCode == importModel.ImportInfo.WareHouseCode && lstTank.ToArray().Contains(x.TankId));

            if(importModel.HandleDate == null)
            {
                importModel.HandleDate = DateTime.Now.ToString(Constants.DATE_FORMAT); ;
            }

            if (null != manualInput && manualInput == true)
            {
                importModel.ManualInput = true;
            }

            if (null == isIndex || isIndex == false)
            {
                var endDate = DateTime.ParseExact(importModel.HandleDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);

                for (int i = 0; i < importModel.TankImports.Count; i++)
                {
                    importModel.TankImports[i].EndDate = endDate;
                    //temp value
                    importModel.TankImportTemps[i].EndDate = endDate;
                    tankImportService.UpdateTankImport(importModel.TankImports[i]);
                    tankImportTempService.UpdateTankImportTemp(importModel.TankImportTemps[i]);
                }

                importModel.ImportInfo.MTankImport = importModel.TankImports;
                importModel.ImportInfo.MTankImportTemps = importModel.TankImportTemps;
                importService.GetEndHandleImportInfo(importModel.ImportInfo, endDate);

            }

            return View(importModel);
        }

        [HttpPost]
        public ActionResult GetEndHandleInfo(ImportModel importModel)
        {
            var importInfo = importService.GetImportInfo(importModel.ImportInfo.Id);

            if (importInfo != null)
            {
                var endDate = DateTime.ParseExact(importModel.HandleDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);

                for (int i = 0; i < importInfo.MTankImport.Count; i++)
                {
                    importInfo.MTankImport[i].EndDate = endDate;

                    //log value
                    importInfo.MTankImportTemps[i].EndDate = endDate;
                }

                importService.GetEndHandleImportInfo(importInfo, endDate);
            }

            return RedirectToAction("EndHandle", new { id = importModel.ImportInfo.Id, isIndex = true , handleDate = importModel.HandleDate });
        }

        [HttpPost]
        public ActionResult DoEndHandle(ImportModel importModel)
        {
            log.Info("Start Import DoEndHandle ");
            var tankImportList = tankImportService.GetTankImportByImportinfoId(byte.Parse(Session[Constants.Session_WareHouse].ToString()), importModel.ImportInfo.Id).ToList();
            var importInfo = importService.GetImportInfo(importModel.ImportInfo.Id);
            //var lstTankImport = tankImportService.GetTankImportByImportinfoId(byte.Parse(Session[Constants.Session_WareHouse].ToString()), importModel.ImportInfo.Id).ToList();
            if (importInfo != null)
            {
                for(int i = 0; i < tankImportList.Count(); i++)
                {
                    tankImportList[i].EndDate = DateTime.ParseExact(importModel.HandleDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                    tankImportService.UpdateTankImport(tankImportList[i]);
                }
                if (Request.Form["submitUpdate"] != null)
                {
                    log.Info("Start Import DoEndHandle submitUpdate ");

                    for (int i = 0; i < importModel.TankImports.Count(); i++)
                    {
                        for (int k = 0; k < tankImportList.Count(); k++)
                        {
                            if (tankImportList[k].TankId == importModel.TankImports[i].TankId)
                            {
                                tankImportList[k].EndTemperature = importModel.TankImports[i].EndTemperature ?? 0;
                                tankImportList[k].EndProductLevel = importModel.TankImports[i].EndProductLevel ?? 0;
                                tankImportList[k].EndDensity = importModel.TankImports[i].EndDensity ?? 0;
                                tankImportList[k].EndDate = DateTime.ParseExact(importModel.HandleDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                                tankImportService.UpdateTankImport(tankImportList[k]);
                            }
                        }
                        //var tankImport = importModel.TankImports[i];
                        //                  tankImport.EndTemperature = tankImport.EndTemperature ?? 0;
                        //                  tankImport.EndProductLevel = tankImport.EndProductLevel ?? 0;
                        //                  tankImport.EndDensity = tankImport.EndDensity ?? 0;
                        //tankImportService.UpdateTankImport(tankImport);
                    }
                    //Update vào bảng  MExportArmImport Chỉ số đầu của họng
                    if (importModel.ListExportArmImport != null)
                    {
                        for (int j = 0; j < importModel.ListExportArmImport.Count(); j++)
                        {
                            var ob = importModel.ListExportArmImport[j];
                            ob.ImportInfoId = importModel.ImportInfo.Id;
                            ob.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                            exportArmImportService.Update_EndExportArmImport(ob);
                        }
                    }

                    tankImportList = tankImportService.GetTankImportByImportinfoId(byte.Parse(Session[Constants.Session_WareHouse].ToString()), importModel.ImportInfo.Id).ToList();

                    importService.UpdateEndHandleTankImport(tankImportList, importInfo);
                    log.Info("End Import DoEndHandle submitUpdate ");

                    return RedirectToAction("EndHandle", new { id = importModel.ImportInfo.Id, manualInput = true, isIndex = true, handleDate = importModel.HandleDate });
                }
                else if (Request.Form["submitFinish"] != null)
                {
                    var rs = false;
                    log.Info("Start Import DoEndHandle submitFinish ");

                    if (importModel.ManualInput == true)
                    {
                        var message = importModel.ManualMessage;

                        rs = importService.FinishEndHandleImportInfoManual(importInfo, HttpContext.User.Identity.Name, message);

                        for (int i = 0; i < importModel.TankImports.Count(); i++)
                        {
                            var tankImport = importModel.TankImports[i];
                            importModel.TankImports[i].EndTemperature = tankImport.EndTemperature ?? 0;
                            importModel.TankImports[i].EndProductLevel = tankImport.EndProductLevel ?? 0;
                            importModel.TankImports[i].EndDensity = tankImport.EndDensity ?? 0;
                            importModel.TankImports[i].EndDate = DateTime.ParseExact(importModel.HandleDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                            tankImportService.UpdateTankImport(importModel.TankImports[i]);
                        }

                        //Update vào bảng  MExportArmImport Chỉ số đầu của họng
                        if (importModel.ListExportArmImport != null)
                        {
                            for (int j = 0; j < importModel.ListExportArmImport.Count(); j++)
                            {
                                var ob = importModel.ListExportArmImport[j];
                                ob.ImportInfoId = importModel.ImportInfo.Id;
                                ob.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                                exportArmImportService.Update_EndExportArmImport(ob);
                            }
                        }

                    }
                    else
                    {
                        rs = importService.FinishEndHandleImportInfo(importInfo, HttpContext.User.Identity.Name);

                        //Update vào bảng  MExportArmImport Chỉ số đầu của họng
                        if (importModel.ListExportArmImport != null)
                        {
                            for (int j = 0; j < importModel.ListExportArmImport.Count(); j++)
                            {
                                var ob = importModel.ListExportArmImport[j];
                                ob.ImportInfoId = importModel.ImportInfo.Id;
                                ob.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                                exportArmImportService.Update_EndExportArmImport(ob);
                            }
                        }

                    }


                    //Update Vtt và V15 đưa vào bảng MImportInfo
                    var startSum = 0.0;
                    var endSum = 0.0;
                    var startSumVtt = 0.0;
                    var endSumVtt = 0.0;
                    var endSumV15 = 0.0;
                    importModel.TankImportInfoData = reportService.TankImportInfoData(importModel.ImportInfo.Id);
                    importModel.ExportArmImportData = reportService.ExportArmImportData(importModel.ImportInfo.Id);
                    importModel.TankImportData = reportService.TankImportData(importModel.ImportInfo.Id);

                    foreach (System.Data.DataRow row in importModel.TankImportData.Rows)
                    {
                        if (@Convert.ToInt32(row["TankId"]) != 9999) //bo qua Tec CC
                        {
                            startSum += ((row["StartTemperature"] == @DBNull.Value) ? 0 : @Convert.ToDouble(row["StartTemperature"])) * ((row["StartProductVolume"] == @DBNull.Value) ? 0 : @Convert.ToDouble(row["StartProductVolume"]));
                            endSum += ((row["EndTemperature"] == @DBNull.Value) ? 0 : @Convert.ToDouble(row["EndTemperature"])) * ((row["EndProductVolume"] == @DBNull.Value) ? 0 : @Convert.ToDouble(row["EndProductVolume"]));
                            startSumVtt += (row["StartProductVolume"] == @DBNull.Value) ? 0 : @Convert.ToDouble(row["StartProductVolume"]);
                            endSumVtt += (row["EndProductVolume"] == @DBNull.Value) ? 0 : @Convert.ToDouble(row["EndProductVolume"]);
                            endSumV15 += (row["EndProductVolume15"] == @DBNull.Value) ? 0 : @Convert.ToDouble(row["EndProductVolume15"]);
                        }
                    }

                    double sumStartVTT = 0.0, sumEndVTT = 0.0, sumStartV15 = 0.0, sumEndV15 = 0.0, sumStartTemp = 0.0, sumEndTemp = 0.0, sumEndVCF = 0.0;
                    double.TryParse(importModel.TankImportData.Compute("Sum(StartProductVolume)", "").ToString(), out sumStartVTT);
                    double.TryParse(importModel.TankImportData.Compute("Sum(EndProductVolume)", "").ToString(), out sumEndVTT);
                    double.TryParse(importModel.TankImportData.Compute("Sum(StartProductVolume15)", "").ToString(), out sumStartV15);
                    double.TryParse(importModel.TankImportData.Compute("Sum(EndProductVolume15)", "").ToString(), out sumEndV15);
                    double.TryParse(importModel.TankImportData.Compute("Sum(EndVCF)", "").ToString(), out sumEndVCF);

                    if (sumStartVTT != 0)
                    {
                        sumStartTemp = startSum / startSumVtt;
                    }

                    if (sumEndVTT != 0)
                    {
                        sumEndTemp = endSum / endSumVtt;
                    }

                    double sumExportV = 0.0, sumExportV15 = 0.0;
                    double.TryParse(importModel.ExportArmImportData.Compute("Sum(ExportValue)", "").ToString(), out sumExportV);
                    double.TryParse(importModel.ExportArmImportData.Compute("Sum(ExportValue15)", "").ToString(), out sumExportV15);

                    var chenhlech = sumEndTemp - sumStartTemp;
                    //var vtung = (chenhlech * 0.0009 * sumStartVTT);

                    //var ketluanVtt = sumEndVTT + sumExportV - (sumStartVTT + vtung);
                    var ketluanVtt = sumEndVTT + sumExportV - (sumStartVTT);
                    var ketluanV15 = sumEndV15 + sumExportV15 - sumStartV15;

                    importInfo.V15_Actual = (float)ketluanV15;
                    importInfo.V_Actual = (float)ketluanVtt;
                    importService.UpdateImportInfo(importInfo);

                    if (rs)
                    {
                        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                    }
                    else
                    {
                        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
                    }
                    log.Info("557");
                    log.Info("End Import DoEndHandle submitFinish ");
                }
            }

            log.Info("End Import DoEndHandle ");
            return RedirectToAction("EndHandle", new { id = importModel.ImportInfo.Id, isIndex = true , handleDate = importModel.HandleDate });
        }

        public ActionResult Export(int id)
        {
            importModel.ImportInfo = importService.GetImportInfo(id);
            //ThangNK comment tam thoi
            //importModel.TankList = importModel.ImportInfo.MTankImport
            //    .Where(ti => ti.TankId != Constants.TEC_CC_ID).Select(ti => new SelectListItem
            //    {
            //        Value = ti.TankId.ToString(),
            //        Text = ti.MTank.TankName
            //    }).ToList();


            var lstTankIdTankImport = tankImportService.GetTankImportByImportinfoId(byte.Parse(Session[Constants.Session_WareHouse].ToString()), id);
            List<int> lstTankId = new List<int>();

            foreach (var item in lstTankIdTankImport)
            {
                lstTankId.Add(item.TankId);
            }
            var listTankName = tankService.GetAllTank().Where(x => x.WareHouseCode == importModel.ImportInfo.WareHouseCode && lstTankId.Contains(x.TankId)).Select(x => x.TankName).ToArray();



            string nameList = String.Empty;
            foreach (var name in listTankName)
            {
                nameList += name + ", ";
            }
            importModel.ListTankName = nameList.Remove(nameList.Length - 2, 2);

            importModel.TankList = tankService.GetAllTank().Where(x => x.WareHouseCode == importModel.ImportInfo.WareHouseCode && lstTankId.Contains(x.TankId));
            importModel.ListClock = clockService.GetAllClock().ToList();
            // var lstClockExport = new List<MClockExport>();
            importModel.TankId = importService.GetExportTankId(id);
            importModel.ExportVtt = importService.GetExportVtt(id);
            importModel.ExportV15 = importService.GetExportV15(id);

            return View(importModel);
        }

        [HttpPost]
        public ActionResult DoExport(ImportModel importModel)
        {
            var importInfo = importService.GetImportInfo(importModel.ImportInfo.Id);

            if (importInfo != null)
            {
                for (int i = 0; i < importModel.ListClockExport.Count(); i++)
                {

                    if (importModel.ListClockExport[i].StartVtt != null && importModel.ListClockExport[i].EndVtt != null)
                    {
                        if ((double)importModel.ListClockExport[i].StartVtt <= (double)importModel.ListClockExport[i].EndVtt)
                        {
                            importService.Update_ClockExport_StarVtt_EndVtt(importModel.ImportInfo.Id, i + 1, (double)importModel.ListClockExport[i].StartVtt, (double)importModel.ListClockExport[i].EndVtt);
                        }
                    }

                }

                var rs = importService.ClockExport(importInfo, importModel.TankId,
                    importModel.ExportVtt, importModel.ExportV15, HttpContext.User.Identity.Name);

                if (rs)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                }
                else
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
                }
            }

            return RedirectToAction("Edit", new { id = importModel.ImportInfo.Id });
        }


        public JsonResult GetConfigArm(int productId)
        {
            var obj = productService.GetAllProduct().Where(x => x.ProductId == productId).FirstOrDefault();
            if (obj == null)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var lst = configArmService.GetAllConfigArm().Where(x => x.WareHouseCode == byte.Parse(Session[Constants.Session_WareHouse].ToString()) && (x.ProductCode_1 == obj.ProductCode || x.ProductCode_2 == obj.ProductCode || x.ProductCode_2 == obj.ProductCode));
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetProductById(int productId)
        {
            var obj = productService.GetAllProduct().Where(x => x.ProductId == productId).FirstOrDefault();
            if (obj == null)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var objprd = new ProductTemp();
                objprd.Abbreviations = obj.Abbreviations;
                objprd.ExportWastageRate = obj.ExportWastageRate;
                objprd.InputWastageRate = obj.InputWastageRate;
                objprd.ProductCode = obj.ProductCode;
                objprd.ProductName = obj.ProductName;
                return Json(objprd, JsonRequestBehavior.AllowGet);
            }
        }


    }
}