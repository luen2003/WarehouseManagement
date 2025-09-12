using PagedList;
using PetroBM.Common.Util;
using PetroBM.Web.Models;
using PetroBM.Services.Services;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetroBM.Domain.Entities;
using System.Web.Script.Serialization;
using System.Globalization;
using PetroBM.Web.Attribute;
using log4net;
using Newtonsoft.Json;

namespace PetroBM.Web.Controllers
{
    [HasPermission(Constants.PERMISSION_CHART)]
    public class ChartController : BaseController
    {
        private readonly ITankService tankService;
        private readonly IProductService productService;
        private readonly ITankGroupService tankGroupService;
        private readonly IWareHouseService warehouseService;
        private readonly IConfigArmService configarmService;
        private readonly ILiveDataArmService livedataarmService;
        private readonly ITankLiveService tankliveService;
        private readonly IChartService chartService;
        private readonly ITankGrpTankService tankGrpTankService;
        private readonly BaseService baseService;

        public ChartModel chartModel;

        public ChartController(ITankService tankService, IProductService productService,
            ITankGroupService tankGroupService, ChartModel chartModel, IChartService chartService,
            IWareHouseService warehouseService, IConfigArmService configarmService,
            ILiveDataArmService livedataarmService, ITankLiveService tankliveService, IReportService reportService, ITankGrpTankService tankGrpTankService, BaseService baseService)
        {
            this.tankService = tankService;
            this.productService = productService;
            this.tankGroupService = tankGroupService;
            this.chartModel = chartModel;
            this.warehouseService = warehouseService;
            this.configarmService = configarmService;
            this.livedataarmService = livedataarmService;
            this.tankliveService = tankliveService;
            this.chartService = chartService;
            this.tankGrpTankService = tankGrpTankService;
            this.baseService = baseService;

        }

        /// <summary>
        /// Thời gian thực đo bể
        /// </summary>
        /// <param name="chartModel"></param>
        /// <returns></returns>
        public ActionResult LiveChart(ChartModel chartModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CHART_LIVECHART);
            if (checkPermission)
            {
                chartModel.WareHouseList = warehouseService.GetAllWareHouse();
                chartModel.TankTempList = tankService.GetAllTankOrderByName().Select(c => new TankTempModel
                {
                    ProductId = (int)c.ProductId,
                    TankId = c.TankId,
                    WareHouseCode = (byte)c.WareHouseCode,
                    TankName = c.TankName
                }).ToList();
                chartModel.DurationList = GetDurationList();
                return View(chartModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        /// <summary>
        /// Thời gian thực bến xuất
        /// </summary>
        /// <param name="chartModel"></param>
        /// <returns></returns>
        public ActionResult LiveChartExport(ChartModel chartModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CHART_LIVECHARTEXPORT);
            if (checkPermission)
            {
                chartModel.DurationList = GetDurationList();
                chartModel.WareHouseList = warehouseService.GetAllWareHouse();
                chartModel.ConfigArmTempList = configarmService.GetAllConfigArmOrderByName().Select(c => new ConfigArmTempModel
                {
                    ArmName = c.ArmName,
                    ArmNo = c.ArmNo,
                    ConfigArmId = c.ConfigArmID,
                    WareHouseCode = c.WareHouseCode
                }).ToList();

                chartModel.DurationList = GetDurationList();
                return View(chartModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        private IEnumerable<object> GetDurationList()
        {
            var durationList = new List<object>();
            durationList.Add(new { Value = "1", Text = "1 phút" });
            durationList.Add(new { Value = "5", Text = "5 phút" });
            durationList.Add(new { Value = "10", Text = "10 phút" });
            durationList.Add(new { Value = "15", Text = "15 phút" });
            durationList.Add(new { Value = "30", Text = "30 phút" });
            durationList.Add(new { Value = "60", Text = "60 phút" });
            durationList.Add(new { Value = "120", Text = "120 phút" });

            return durationList;
        }

        #region "Xử lí trực tuyển theo bể"

        [HttpPost]
        public ActionResult GetLiveTotalLevel(ChartModel chartModel)
        {
            float? min = null;
            float? max = null;

            if (chartModel.IsTotalLevel)
            {
                var time = DateTime.Now.AddMinutes(-chartModel.Duration);
                var tankLiveList = (List<MTankLive>)HttpContext.Application["TankLiveList"];
                var logList = tankLiveList.Where(tl => (tl.TankId == chartModel.TankId) && (tl.InsertDate >= time) && tl.WareHouseCode == chartModel.WareHouseCode);

                if (logList.Count() > 0)
                {
                    foreach (var log in logList)
                    {
                        chartModel.TotalLevelData.Add(new DataPoint(DateTimeUtil.GetJSTime(log.InsertDate), log.TotalLevel));
                    }
                }

                min = chartModel.TotalLevelData.Min(pl => pl.y);
                max = chartModel.TotalLevelData.Max(pl => pl.y);
            }

            return Json(new { min = NumberUtil.FormatNumber(min, 2), max = NumberUtil.FormatNumber(max, 2), data = chartModel.TotalLevelData });
        }

        [HttpPost]
        public ActionResult GetLiveProductVolume(ChartModel chartModel)
        {
            float? min = null;
            float? max = null;

            if (chartModel.IsProductVolume)
            {

                var time = DateTime.Now.AddMinutes(-chartModel.Duration);
                var tankLiveList = (List<MTankLive>)HttpContext.Application["TankLiveList"];
                var logList = tankLiveList.Where(tl => (tl.TankId == chartModel.TankId) && (tl.InsertDate >= time) && (tl.WareHouseCode == (byte)chartModel.WareHouseCode));

                if (logList.Count() > 0)
                {
                    foreach (var log in logList)
                    {
                        chartModel.ProductVolumeData
                            .Add(new DataPoint(DateTimeUtil.GetJSTime(log.InsertDate), (float?)log.ProductVolume));
                    }
                }

                min = chartModel.ProductVolumeData.Min(pl => pl.y);
                max = chartModel.ProductVolumeData.Max(pl => pl.y);
            }

            return Json(new { min = NumberUtil.FormatNumber(min, 2), max = NumberUtil.FormatNumber(max, 2), data = chartModel.ProductVolumeData });
        }

        [HttpPost]
        public ActionResult GetLiveFlowRate(ChartModel chartModel)
        {
            float? min = null;
            float? max = null;

            if (chartModel.IsFlowRate)
            {
                var time = DateTime.Now.AddMinutes(-chartModel.Duration);
                var tankLiveList = (List<MTankLive>)HttpContext.Application["TankLiveList"];
                var logList = tankLiveList.Where(tl => (tl.TankId == chartModel.TankId) && (tl.InsertDate >= time) && (tl.WareHouseCode == (byte)chartModel.WareHouseCode));

                if (logList.Count() > 0)
                {
                    foreach (var log in logList)
                    {
                        chartModel.FlowRateData
                            .Add(new DataPoint(DateTimeUtil.GetJSTime(log.InsertDate), (float?)log.FlowRate));
                    }
                }

                min = chartModel.FlowRateData.Min(pl => pl.y);
                max = chartModel.FlowRateData.Max(pl => pl.y);
            }

            return Json(new { min = NumberUtil.FormatNumber(min, 2), max = NumberUtil.FormatNumber(max, 2), data = chartModel.FlowRateData });
        }

        [HttpPost]
        public ActionResult GetLiveAvgTemperature(ChartModel chartModel)
        {
            float? min = null;
            float? max = null;

            if (chartModel.IsAvgTemperature)
            {
                var time = DateTime.Now.AddMinutes(-chartModel.Duration);
                var tankLiveList = (List<MTankLive>)HttpContext.Application["TankLiveList"];
                var logList = tankLiveList.Where(tl => (tl.TankId == chartModel.TankId) && (tl.InsertDate >= time) && (tl.WareHouseCode == (byte)chartModel.WareHouseCode));

                if (logList.Count() > 0)
                {
                    foreach (var log in logList)
                    {
                        chartModel.AvgTemperatureData
                            .Add(new DataPoint(DateTimeUtil.GetJSTime(log.InsertDate), log.AvgTemperature));
                    }
                }

                min = chartModel.AvgTemperatureData.Min(pl => pl.y);
                max = chartModel.AvgTemperatureData.Max(pl => pl.y);
            }

            return Json(new { min = NumberUtil.FormatNumber(min, 1), max = NumberUtil.FormatNumber(max, 1), data = chartModel.AvgTemperatureData });
        }


        #endregion

        #region "Xử lí trực tuyển họng"
        // Xử lí trực tuyến Họng *************************************************************************************************************************

        [HttpPost]
        public ActionResult GetLiveDataArmFlowRateBase(ChartModel chartModel)
        {
            float? min = null;
            float? max = null;

            if (chartModel.IsFlowRateBase)
            {
                var time = DateTime.Now.AddMinutes(-chartModel.Duration);
                var liveDataArmList = (List<MLiveDataArm>)HttpContext.Application["LiveDataArmList"];
                var logList = liveDataArmList.Where(tl => (tl.ArmNo == chartModel.ArmNo) && (tl.TimeLog >= time) && tl.WareHouseCode == chartModel.WareHouseCode);

                if (logList.Count() > 0)
                {
                    chartModel.FlowRateBaseData = new List<DataPoint>();
                    foreach (var log in logList)
                    {
                        chartModel.FlowRateBaseData.Add(new DataPoint(DateTimeUtil.GetJSTime(log.TimeLog), log.Flowrate_Base));
                    }
                }

                if (chartModel.FlowRateBaseData != null)
                {
                    min = chartModel.FlowRateBaseData.Min(pl => pl.y);
                    max = chartModel.FlowRateBaseData.Max(pl => pl.y);
                }
                else
                {
                    min = 0;
                    max = 0;
                }


            }

            return Json(new { min = NumberUtil.FormatNumber(min, 2), max = NumberUtil.FormatNumber(max, 2), data = chartModel.FlowRateBaseData });
        }

        public JsonResult GetHistoricalFlowRateBaseData(string startdate, string enddate, byte wareHouseCode, byte armNo, string productCode)
        {
            float dmin = 99999999999999999;
            float dmax = 0;
            DateTime strstartdate = DateTime.ParseExact(startdate, Constants.DATE_FORMAT, CultureInfo.CurrentCulture);
            DateTime strenddate = DateTime.ParseExact(enddate, Constants.DATE_FORMAT, CultureInfo.CurrentCulture);
            chartModel.TotalLevelData = new List<DataPoint>();
            chartModel.DataTable = chartService.Char_HistoricalExportDataLogArm(strstartdate, strenddate, wareHouseCode, armNo, productCode, 0);

            foreach (DataRow row in chartModel.DataTable.Rows)
            {
                float V_Actual = float.Parse(row["Flowrate_Base"].ToString());

                if (V_Actual < dmin)
                    dmin = V_Actual;

                if (V_Actual > dmax)
                    dmax = V_Actual;

                DateTime date = DateTime.Parse(row["TimeLog"].ToString());
                chartModel.TotalLevelData.Add(new DataPoint(DateTimeUtil.GetJSTime(date), V_Actual));

            }
            if (dmin > dmax)
            {
                dmin = 0;
            }
            return Json(new { min = dmin, max = dmax, data = chartModel.TotalLevelData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHistoricalFlowRateEthanolData(string startdate, string enddate, byte wareHouseCode, byte armNo, string productCode)
        {
            float dmin = 99999999999999999;
            float dmax = 0;
            DateTime strstartdate = DateTime.ParseExact(startdate, Constants.DATE_FORMAT, CultureInfo.CurrentCulture);
            DateTime strenddate = DateTime.ParseExact(enddate, Constants.DATE_FORMAT, CultureInfo.CurrentCulture);
            chartModel.TotalLevelData = new List<DataPoint>();
            chartModel.DataTable = chartService.Char_HistoricalExportDataLogArm(strstartdate, strenddate, wareHouseCode, armNo, productCode, 0);

            foreach (DataRow row in chartModel.DataTable.Rows)
            {
                float V_Actual = float.Parse(row["Flowrate_E"].ToString());

                if (V_Actual < dmin)
                    dmin = V_Actual;

                if (V_Actual > dmax)
                    dmax = V_Actual;

                DateTime date = DateTime.Parse(row["TimeLog"].ToString());
                chartModel.TotalLevelData.Add(new DataPoint(DateTimeUtil.GetJSTime(date), V_Actual));

            }
            if (dmin > dmax)
            {
                dmin = 0;
            }
            return Json(new { min = dmin, max = dmax, data = chartModel.TotalLevelData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHistoricalFlowRateData(string startdate, string enddate, byte wareHouseCode, byte armNo, string productCode)
        {
            float dmin = 99999999999999999;
            float dmax = 0;
            DateTime strstartdate = DateTime.ParseExact(startdate, Constants.DATE_FORMAT, CultureInfo.CurrentCulture);
            DateTime strenddate = DateTime.ParseExact(enddate, Constants.DATE_FORMAT, CultureInfo.CurrentCulture);
            chartModel.TotalLevelData = new List<DataPoint>();
            chartModel.DataTable = chartService.Char_HistoricalExportDataLogArm(strstartdate, strenddate, wareHouseCode, armNo, productCode, 0);
            foreach (DataRow row in chartModel.DataTable.Rows)
            {
                float V_Actual = float.Parse(row["Flowrate"].ToString());

                if (V_Actual < dmin)
                    dmin = V_Actual;

                if (V_Actual > dmax)
                    dmax = V_Actual;

                DateTime date = DateTime.Parse(row["TimeLog"].ToString());
                chartModel.TotalLevelData.Add(new DataPoint(DateTimeUtil.GetJSTime(date), V_Actual));

            }
            if (dmin > dmax)
            {
                dmin = 0;
            }
            return Json(new { min = dmin, max = dmax, data = chartModel.TotalLevelData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHistoricalActualRatioData(string startdate, string enddate, byte wareHouseCode, byte armNo, string productCode)
        {
            float dmin = 99999999999999999;
            float dmax = 0;
            DateTime strstartdate = DateTime.ParseExact(startdate, Constants.DATE_FORMAT, CultureInfo.CurrentCulture);
            DateTime strenddate = DateTime.ParseExact(enddate, Constants.DATE_FORMAT, CultureInfo.CurrentCulture);
            chartModel.TotalLevelData = new List<DataPoint>();
            chartModel.DataTable = chartService.Char_HistoricalExportDataLogArm(strstartdate, strenddate, wareHouseCode, armNo, productCode, 0);

            foreach (DataRow row in chartModel.DataTable.Rows)
            {
                float V_Actual = float.Parse(row["ActualRatio"].ToString());

                if (V_Actual < dmin)
                    dmin = V_Actual;

                if (V_Actual > dmax)
                    dmax = V_Actual;

                DateTime date = DateTime.Parse(row["TimeLog"].ToString());
                chartModel.TotalLevelData.Add(new DataPoint(DateTimeUtil.GetJSTime(date), V_Actual));
            }
            if (dmin > dmax)
            {
                dmin = 0;
            }
            return Json(new { min = dmin, max = dmax, data = chartModel.TotalLevelData }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetLiveDataArmFlowRateBaseE(ChartModel chartModel)
        {
            float? min = 0;
            float? max = 0;

            if (chartModel.IsFlowRateE)
            {
                var time = DateTime.Now.AddMinutes(-chartModel.Duration);
                var liveDataArmList = (List<MLiveDataArm>)HttpContext.Application["LiveDataArmList"];
                var logList = liveDataArmList.Where(tl => (tl.ArmNo == chartModel.ArmNo) && (tl.TimeLog >= time) && tl.WareHouseCode == chartModel.WareHouseCode);

                if (logList.Count() > 0)
                {
                    chartModel.FlowRateEData = new List<DataPoint>();
                    foreach (var log in logList)
                    {
                        chartModel.FlowRateEData.Add(new DataPoint(DateTimeUtil.GetJSTime(log.TimeLog), log.Flowrate_E));
                    }
                }
                if (chartModel.FlowRateEData != null)
                {
                    min = chartModel.FlowRateEData.Min(pl => pl.y);
                    max = chartModel.FlowRateEData.Max(pl => pl.y);
                }
                else
                {
                    min = 0;
                    max = 0;
                }

            }

            return Json(new { min = NumberUtil.FormatNumber(min, 2), max = NumberUtil.FormatNumber(max, 2), data = chartModel.FlowRateEData });
        }


        [HttpPost]
        public ActionResult GetLiveDataArmFlowRate(ChartModel chartModel)
        {
            float? min = 0;
            float? max = 0;

            if (chartModel.IsFlowRate)
            {
                var time = DateTime.Now.AddMinutes(-chartModel.Duration);
                var liveDataArmList = (List<MLiveDataArm>)HttpContext.Application["LiveDataArmList"];
                var logList = liveDataArmList.Where(tl => (tl.ArmNo == chartModel.ArmNo) && (tl.TimeLog >= time) && tl.WareHouseCode == chartModel.WareHouseCode);

                if (logList.Count() > 0)
                {
                    chartModel.FlowRateData = new List<DataPoint>();
                    foreach (var log in logList)
                    {
                        chartModel.FlowRateData.Add(new DataPoint(DateTimeUtil.GetJSTime(log.TimeLog), log.Flowrate));
                    }
                }

                if (chartModel.FlowRateData != null)
                {
                    min = chartModel.FlowRateData.Min(pl => pl.y);
                    max = chartModel.FlowRateData.Max(pl => pl.y);
                }
                else
                {
                    min = 0;
                    max = 0;
                }

            }

            return Json(new { min = NumberUtil.FormatNumber(min, 2), max = NumberUtil.FormatNumber(max, 2), data = chartModel.FlowRateData });
        }

        [HttpPost]
        public ActionResult GetLiveDataArmActualRatio(ChartModel chartModel)
        {
            float? min = null;
            float? max = null;

            if (chartModel.IsActualRatio)
            {
                var time = DateTime.Now.AddMinutes(-chartModel.Duration);
                var liveDataArmList = (List<MLiveDataArm>)HttpContext.Application["LiveDataArmList"];
                var logList = liveDataArmList.Where(tl => (tl.ArmNo == chartModel.ArmNo) && (tl.TimeLog >= time) && tl.WareHouseCode == chartModel.WareHouseCode);

                if (logList.Count() > 0)
                {
                    chartModel.ActualRatioData = new List<DataPoint>();
                    foreach (var log in logList)
                    {
                        chartModel.ActualRatioData.Add(new DataPoint(DateTimeUtil.GetJSTime(log.TimeLog), log.ActualRatio));
                    }
                }

                if (chartModel.ActualRatioData != null)
                {
                    min = chartModel.ActualRatioData.Min(pl => pl.y);
                    max = chartModel.ActualRatioData.Max(pl => pl.y);
                }
                else
                {
                    min = 0;
                    max = 0;
                }



            }

            return Json(new { min = NumberUtil.FormatNumber(min, 2), max = NumberUtil.FormatNumber(max, 2), data = chartModel.ActualRatioData });
        }

        //*************************************************************************************************************************************************************

        #endregion

        public JsonResult GetDiffExportData(string startdate, string enddate, byte wareHouseCode, double deviation, string productCode)
        {
            float dmin = float.MaxValue;
            float dmax = float.MinValue;
            float davg = 0;
            float sum = 0;
            float absavg;
            int count = 0;

            DateTime strstartdate = DateTime.ParseExact(startdate, Constants.DATE_FORMAT, CultureInfo.CurrentCulture);
            DateTime strenddate = DateTime.ParseExact(enddate, Constants.DATE_FORMAT, CultureInfo.CurrentCulture);
            chartModel.DiffExportData = new List<DataList>();
            chartModel.DataTable = chartService.Chart_DiffExportDay(strstartdate, strenddate, wareHouseCode, productCode, deviation);
            //var deviationList = (List<MCommandDetail>)HttpContext.Application["CommandDetailList"];
            //var listData = deviationList.Where(tl => (tl.WareHouseCode == wareHouseCode) && (tl.ProductCode == productCode) && (tl.TimeStart <= strstartdate) && (tl.TimeStop >= strenddate) && (tl.DeleteFlg == false) && tl.V_Deviation == deviation);

            foreach (DataRow row in chartModel.DataTable.Rows)
            {
                float SumDeviation = float.Parse(row["SumDeviation"].ToString());
                count++;
                sum += SumDeviation;

                if (SumDeviation < dmin)
                    dmin = SumDeviation;

                if (SumDeviation > dmax)
                    dmax = SumDeviation;

                var date = row["Date"].ToString();
                chartModel.DiffExportData.Add(new DataList(date, SumDeviation));
            }
            davg = sum / count;
            absavg = Math.Abs(davg);
            return Json(new { min = dmin, max = dmax, davg = davg, absavg = absavg, data = chartModel.DiffExportData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetErrorExportData(string startdate, string enddate, byte wareHouseCode, double deviation, byte armNo)
        {
            float dmin = float.MaxValue;
            float dmax = float.MinValue;
            float davg = 0;
            float sum = 0;
            float absavg;
            int count = 0;

            DateTime strstartdate = DateTime.ParseExact(startdate, Constants.DATE_FORMAT, CultureInfo.CurrentCulture);
            DateTime strenddate = DateTime.ParseExact(enddate, Constants.DATE_FORMAT, CultureInfo.CurrentCulture);
            chartModel.ErrorExportData = new List<DataList>();
            chartModel.DataTable = chartService.Chart_ExportError(strstartdate, strenddate, wareHouseCode, armNo, deviation);
            //var deviationList = (List<MCommandDetail>)HttpContext.Application["CommandDetailList"];
            //var listData = deviationList.Where(tl => (tl.WareHouseCode == wareHouseCode) && (tl.ProductCode == productCode) && (tl.TimeStart <= strstartdate) && (tl.TimeStop >= strenddate) && (tl.DeleteFlg == false) && tl.V_Deviation == deviation);

            foreach (DataRow row in chartModel.DataTable.Rows)
            {
                float V_Deviation = float.Parse(row["V_Deviation"].ToString());
                count++;
                sum += V_Deviation;

                if (V_Deviation < dmin)
                    dmin = V_Deviation;

                if (V_Deviation > dmax)
                    dmax = V_Deviation;

                var date = row["TimeOrder"].ToString();
                chartModel.ErrorExportData.Add(new DataList(date, V_Deviation));
            }
            davg = sum / count;
            absavg = Math.Abs(davg);
            //return Json(new { min = dmin, max = dmax, davg = davg, absavg = absavg, data = chartModel.ErrorExportData }, JsonRequestBehavior.AllowGet);
            return Json(new { min = dmin, max = dmax, data = chartModel.ErrorExportData }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Lịch sử đo bể
        /// </summary>
        /// <param name="chartModel"></param>
        /// <returns></returns>
        public ActionResult HistoricalChart(ChartModel chartModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CHART_HISTORICALCHART);
            if (checkPermission)
            {
                chartModel.TankTempList = tankService.GetAllTankOrderByName().Select(c => new TankTempModel
                {
                    ProductId = (int)c.ProductId,
                    TankId = c.TankId,
                    WareHouseCode = (byte)c.WareHouseCode,
                    TankName = c.TankName
                }).ToList();
                chartModel.WareHouseList = warehouseService.GetAllWareHouse();
                DateTime? startDate = null;
                DateTime? endDate = null;

                if (chartModel.WareHouseCode == null)
                {
                    if (Session[Constants.Session_WareHouse] == null)
                    {
                        if (chartModel.WareHouseList.Count() > 0)
                        {
                            foreach (var it in chartModel.WareHouseList)
                            {
                                chartModel.WareHouseCode = (byte)it.WareHouseCode;
                                break;
                            }
                        }

                    }
                    else
                    {
                        chartModel.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                    }
                }

                if (!String.IsNullOrEmpty(chartModel.StartDate))
                {
                    startDate = DateTime.ParseExact(chartModel.StartDate6h, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                if (!String.IsNullOrEmpty(chartModel.EndDate))
                {
                    endDate = DateTime.ParseExact(chartModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (startDate.HasValue)
                {
                    var dt = DateTime.ParseExact(chartModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                    var compare = DateTime.Compare(dt, DateTime.ParseExact(chartModel.StartDate6h, Constants.DATE_FORMAT, CultureInfo.InvariantCulture));
                    if (compare < 0)
                    {
                        startDate = DateTime.ParseExact(chartModel.StartDate6h, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                        chartModel.StartDate = ((DateTime)startDate).ToString(Constants.DATE_FORMAT);
                    }
                }
                var logList = tankService.GetTankLogByTime(chartModel.WareHouseCode, chartModel.TankId, startDate, endDate);

                if (logList.Count() > 0)
                {
                    chartModel.CanDrawChart = true;
                    foreach (var log in logList)
                    {
                        if (chartModel.IsTotalLevel)
                        {
                            chartModel.TotalLevelData
                              .Add(new DataPoint(DateTimeUtil.GetJSTime(log.InsertDate), log.TotalLevel));
                        }
                        if (chartModel.IsProductVolume)
                        {
                            chartModel.ProductVolumeData
                            .Add(new DataPoint(DateTimeUtil.GetJSTime(log.InsertDate), (float?)log.ProductVolume));
                        }
                        if (chartModel.IsFlowRate)
                        {
                            chartModel.FlowRateData
                            .Add(new DataPoint(DateTimeUtil.GetJSTime(log.InsertDate), (float?)log.FlowRate));
                        }
                        if (chartModel.IsAvgTemperature)
                        {
                            chartModel.AvgTemperatureData
                            .Add(new DataPoint(DateTimeUtil.GetJSTime(log.InsertDate), log.AvgTemperature));
                        }
                    }

                    chartModel.TotalLevelMin = chartModel.TotalLevelData.Min(pl => pl.y);
                    chartModel.TotalLevelMax = chartModel.TotalLevelData.Max(pl => pl.y);
                    chartModel.ProductVolumeMin = chartModel.ProductVolumeData.Min(pl => pl.y);
                    chartModel.ProductVolumeMax = chartModel.ProductVolumeData.Max(pl => pl.y);
                    chartModel.FlowRateMin = chartModel.FlowRateData.Min(pl => pl.y);
                    chartModel.FlowRateMax = chartModel.FlowRateData.Max(pl => pl.y);
                    chartModel.AvgTemperatureMin = chartModel.AvgTemperatureData.Min(pl => pl.y);
                    chartModel.AvgTemperatureMax = chartModel.AvgTemperatureData.Max(pl => pl.y);
                }

                return View(chartModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public ActionResult HistoricalDataTable(ChartModel chartModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_REPORT_HISTORICALDATATABLE);
            if (checkPermission)
            {

                chartModel.TankTempList = tankService.GetAllTankOrderByName().Select(c => new TankTempModel
                {
                    ProductId = -1,
                    TankId = c.TankId,
                    TankName = c.TankName,
                    WareHouseCode = c.WareHouseCode
                }).ToList();
                chartModel.WareHouseList = warehouseService.GetAllWareHouse();


                //if (chartModel.WareHouseCode == null)
                //{
                //    foreach (var item in chartModel.WareHouseList) //Lấy mã kho mặc định
                //    {
                //        if (chartModel.WareHouseCode == null)
                //        {
                //            chartModel.WareHouseCode = item.WareHouseCode;
                //            break;
                //        }
                //    }
                //}

                //if (chartModel.TankId == null)
                //{
                //    foreach (var item in chartModel.TankTempList) //Lấy bể mặc định
                //    {
                //        if (chartModel.TankId == null && item.WareHouseCode == chartModel.WareHouseCode)
                //        {
                //            chartModel.TankId = item.TankId;
                //            break;

                //        }
                //    }
                //}

                int pageNumber = (page ?? 1);
                DateTime? startDate = null;
                DateTime? endDate = null;

                if (!String.IsNullOrEmpty(chartModel.StartDate))
                {
                    startDate = DateTime.ParseExact(chartModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!String.IsNullOrEmpty(chartModel.EndDate))
                {
                    endDate = DateTime.ParseExact(chartModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                // Chỉ cho hiển thị 1 trang -> .ToPagedList(pageNumber, Constants.PAGE_SIZE) -> .ToPagedList(pageNumber, 1000);
                chartModel.TankLogList = tankService.GetTankLogByTime(chartModel.WareHouseCode, chartModel.TankId, startDate, endDate)
                .ToPagedList(pageNumber, 10000);

                return View(chartModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public ActionResult UsagePerformance(ChartModel chartModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CHART_USAGEPERFORMANCE);
            if (checkPermission)
            {

                chartModel.WareHouseList = warehouseService.GetAllWareHouse();
                chartModel.TankGroupList = tankGroupService.GetAllTankGrp();
                //chartModel.TankGroupTempList = tankGroupService.GetAllTankGrp().Select(c=>new TankGrp {
                //    tankgrpid =c.TankGrpId,
                //    tankgrpname = c.TanktGrpName,
                //    warehousecode = c.WareHouseCode
                //}).ToList();

                //if (!String.IsNullOrEmpty(chartModel.StartDate) && !String.IsNullOrEmpty(chartModel.EndDate))
                //{
                //    DateTime startDate = DateTime.ParseExact(chartModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                //    DateTime endDate = DateTime.ParseExact(chartModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);

                //    var tankGrp = tankGroupService.FindTankById(chartModel.TankGroupId);
                //    if ((tankGrp != null) && (tankGrp.MTanks.Count > 0))
                //    {
                //        chartModel.CanDrawChart = true;
                //        foreach (var tank in tankGrp.MTanks)
                //        {
                //            var data = new
                //            {
                //                label = tank.TankName,
                //                avgProduct = NumberUtil.FormatNumber(tankService.GetAvgProductVolumeInTank(tank.TankId, startDate, endDate), 2),
                //                volume = NumberUtil.FormatNumber(tank.VolumeMax, 2),
                //                y = tankService.GetUsagePerformanceOfTank(tank.TankId, startDate, endDate)
                //            };
                //            chartModel.TankTurnOverData.Add(data);
                //        }

                //        var repository = new
                //        {
                //            label = "Kho",
                //            avgProduct = NumberUtil.FormatNumber(tankService.GetAvgProductVolumeInRepository(startDate, endDate), 2),
                //            volume = NumberUtil.FormatNumber(tankService.GetRepositoryVolume(), 2),
                //            y = tankService.GetUsagePerformanceOfRepository(startDate, endDate)
                //        };
                //        chartModel.TankTurnOverData.Add(repository);
                //    }
                //}

                chartModel.TankGroupTempList = tankGroupService.GetAllTankGrp().Select(c => new TankGrp
                {
                    tankgrpid = c.TankGrpId,
                    tankgrpname = c.TanktGrpName,
                    warehousecode = c.WareHouseCode
                }).ToList();

                DateTime? startDate = null;
                DateTime? endDate = null;

                if (!String.IsNullOrEmpty(chartModel.StartDate))
                {
                    startDate = DateTime.ParseExact(chartModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!String.IsNullOrEmpty(chartModel.EndDate))
                {
                    endDate = DateTime.ParseExact(chartModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (chartModel.WareHouseList.Count() > 0)
                {
                    foreach (var item in chartModel.WareHouseList)
                    {
                        chartModel.WareHouseCode = item.WareHouseCode;
                        break;
                    }
                }

                //Tín NT :Phải xử lí đoạn này
                var lstExcute = chartService.Chart_UsagePerformance(startDate, endDate, (byte)chartModel.WareHouseCode, chartModel.TankGroupId);
                if (lstExcute.Rows.Count > 0)
                {
                    chartModel.CanDrawChart = true;
                    var DataList = new List<DataList>();
                    float avgUsage = 0;
                    foreach (DataRow row in lstExcute.Rows)
                    {
                        var dataList = new DataList();
                        dataList.label = row["TankName"].ToString();
                        dataList.y = float.Parse(Math.Round((Convert.ToDouble(row["AvgUsage"])) / ((Convert.ToDouble(row["VolumeMax"]) * Convert.ToDouble(row["DateCount"]))) * 100, 2).ToString());
                        avgUsage += float.Parse(Math.Round((Convert.ToDouble(row["AvgUsage"])) / ((Convert.ToDouble(row["VolumeMax"]) * Convert.ToDouble(row["DateCount"]))) * 100, 2).ToString());
                        DataList.Add(dataList);
                    }
                    avgUsage = avgUsage / lstExcute.Rows.Count;
                    var dataAllTank = new DataList();
                    dataAllTank.label = "Tổng bể";
                    dataAllTank.y = avgUsage;
                    DataList.Add(dataAllTank);
                    chartModel.DataList = DataList;
                }

                return View(chartModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public ActionResult TankTurnOver(ChartModel chartModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CHART_TANKTURNOVER);
            if (checkPermission)
            {
                chartModel.WareHouseList = warehouseService.GetAllWareHouse();
                chartModel.TankGroupTempList = tankGroupService.GetAllTankGrp().Select(c => new TankGrp
                {
                    tankgrpid = c.TankGrpId,
                    tankgrpname = c.TanktGrpName,
                    warehousecode = c.WareHouseCode
                }).ToList();
                //chartModel.TankGroupList = tankGroupService.GetAllTankGrp().ToList();

                if (!String.IsNullOrEmpty(chartModel.StartDate) && !String.IsNullOrEmpty(chartModel.EndDate))
                {
                    DateTime startDate = DateTime.ParseExact(chartModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                    DateTime endDate = DateTime.ParseExact(chartModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);

                    var tankGrp = tankGroupService.FindTankById(chartModel.TankGroupId);
                    var tankList = tankGrpTankService.GetAllTankGrpTankByGrpId(chartModel.TankGroupId).ToList();
                    var tankItem = tankService.GetAllTank().ToList();
                    var tankdata = new List<MTank>();
                    for (int i = 0; i < tankItem.Count(); i++)
                    {
                        for (int k = 0; k < tankList.Count(); k++)
                        {
                            if (tankList[k].TankId == tankItem[i].TankId)
                            {
                                tankdata.Add(tankItem[i]);
                            }

                        }
                    }
                    if ((tankGrp != null) && (tankdata.Count() > 0))
                    {
                        chartModel.CanDrawChart = true;
                        foreach (var tank in tankdata)
                        {
                            if (tank.DeleteFlg == Constants.FLAG_OFF)
                            {
                                var data = new
                                {
                                    label = tank.TankName,
                                    export = NumberUtil.FormatNumber(tankService.GetTankExport(tank.TankId, startDate, endDate), 2),
                                    volume = NumberUtil.FormatNumber(tank.VolumeMax, 2),
                                    y = tankService.GetTankTurnOverNumber(tank.TankId, startDate, endDate)
                                };
                                chartModel.TankTurnOverData.Add(data);
                            }
                        }

                        var repository = new
                        {
                            label = "Kho",
                            export = NumberUtil.FormatNumber(tankService.GetRepositoryExport(startDate, endDate), 2),
                            volume = NumberUtil.FormatNumber(tankService.GetRepositoryVolume(), 2),
                            y = tankService.GetRepositoryTurnOverNumber(startDate, endDate)
                        };
                        chartModel.TankTurnOverData.Add(repository);
                    }
                }

                return View(chartModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public ActionResult ProductsKPI(ChartModel chartModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CHART_PRODUCTSKPI);
            if (checkPermission)
            {

                if (!String.IsNullOrEmpty(chartModel.StartDate) && !String.IsNullOrEmpty(chartModel.EndDate))
                {
                    DateTime startDate = DateTime.ParseExact(chartModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                    DateTime endDate = DateTime.ParseExact(chartModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                    chartModel.WareHouseList = warehouseService.GetAllWareHouse();

                    if (chartModel.WareHouseList.Count() > 0)
                    {
                        foreach (var item in chartModel.WareHouseList)
                        {
                            chartModel.WareHouseCode = item.WareHouseCode;
                            break;
                        }
                    }

                    var products = productService.GetAllProduct();
                    if (products.Count() > 0)
                    {
                        chartModel.CanDrawChart = true;
                        double total = 0;

                        foreach (var product in products)
                        {
                            total += tankService.GetProductOut((byte)chartModel.WareHouseCode, product.ProductId, startDate, endDate);
                        }

                        if (total > 0)
                        {
                            foreach (var product in products)
                            {
                                var productOut = tankService.GetProductOut((byte)chartModel.WareHouseCode, product.ProductId, startDate, endDate);
                                var data = new
                                {
                                    label = product.ProductName,
                                    y = Math.Round((productOut / total), 2) * 100,
                                    export = NumberUtil.FormatNumber(productOut, 2),
                                    color = product.Color
                                };
                                chartModel.ProductsKPIData.Add(data);
                            }
                        }
                    }
                }

                return View(chartModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }


        /// <summary>
        /// Đồ thị chênh lệch xuất theo ngày
        /// </summary>
        /// <param name="chartModel"></param>
        /// <returns></returns>
        public ActionResult DiffExportByDay(ChartModel chartModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CHART_DIFFEXPORTBYDAY);
            if (checkPermission)
            {
                chartModel.WareHouseList = warehouseService.GetAllWareHouse();
                //if (chartModel.WareHouseList.Count() > 0)
                //{
                //    foreach (var it in chartModel.WareHouseList)
                //    {
                //        if (chartModel.WareHouseCode == null)
                //        {
                //            chartModel.WareHouseCode = it.WareHouseCode;
                //            break;
                //        }
                //    }
                //}
                if (chartModel.WareHouseCode == null)
                {
                    chartModel.WareHouseCode = 0;
                }
                chartModel.ProductList = productService.GetAllProduct();
                DateTime? startDate = null;
                DateTime? endDate = null;
                if (!String.IsNullOrEmpty(chartModel.StartDate))
                {
                    startDate = DateTime.ParseExact(chartModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!String.IsNullOrEmpty(chartModel.EndDate))
                {
                    endDate = DateTime.ParseExact(chartModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                //Tín NT :Phải xử lí đoạn này

                var lstExcute = chartService.Chart_DiffExportDay(startDate, endDate, (byte)chartModel.WareHouseCode, chartModel.ProductCode, chartModel.Deviation);
                if (lstExcute.Rows.Count > 0)
                {
                    if ((byte)chartModel.WareHouseCode != 0 && chartModel.ProductCode != null)
                    {
                        chartModel.CanDrawChart = true;
                    }
                    //chartModel.CanDrawChart = true;
                    var DataList = new List<DataList>();
                    foreach (DataRow row in lstExcute.Rows)
                    {
                        var dataList = new DataList();
                        dataList.label = row["Date"].ToString();
                        dataList.y = Convert.ToInt32(row["SumDeviation"]);
                        DataList.Add(dataList);
                    }
                    chartModel.DataList = DataList;
                    var lstExcute_2 = chartService.Chart_DiffExportDay_Calculate_Pie_Chart(startDate, endDate, (byte)chartModel.WareHouseCode, chartModel.ProductCode, chartModel.Deviation);
                    if (lstExcute_2.Rows.Count > 0)
                    {
                        var DataList_PieChart = new List<DataList2>();
                        foreach (DataRow row in lstExcute_2.Rows)
                        {
                            var dataList = new DataList2();
                            dataList.indexLabel = row["SumDeviation"].ToString();
                            dataList.y = Convert.ToInt32(row["CountTime"]);
                            DataList_PieChart.Add(dataList);
                        }
                        chartModel.DataList2 = DataList_PieChart;
                    }

                }

                return View(chartModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        /// <summary>
        /// Đồ thị sai số chênh lệch các mẻ xuất
        /// </summary>
        /// <param name="chartModel"></param>
        /// <returns></returns>
        public ActionResult ExportError(ChartModel chartModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CHART_EXPORTERROR);
            if (checkPermission)
            {
                chartModel.WareHouseList = warehouseService.GetAllWareHouse();
                if (chartModel.WareHouseList.Count() > 0)
                {
                    foreach (var it in chartModel.WareHouseList)
                    {
                        if (chartModel.WareHouseCode == null)
                        {
                            chartModel.WareHouseCode = it.WareHouseCode;
                            break;
                        }
                    }

                }

                chartModel.ConfigArmTempList = configarmService.GetAllConfigArmOrderByName().Select(c => new ConfigArmTempModel
                {
                    ArmName = c.ArmName,
                    ArmNo = c.ArmNo,
                    ConfigArmId = c.ConfigArmID,
                    WareHouseCode = c.WareHouseCode
                }).ToList();

                //Lấy mặc định họng
                var lst = configarmService.GetAllConfigArmOrderByName().Where(x => x.WareHouseCode == chartModel.WareHouseCode).FirstOrDefault();
                if (lst != null)
                {
                    if (chartModel.ArmNo == null)
                        chartModel.ArmNo = lst.ArmNo;
                }

                DateTime? startDate = null;
                DateTime? endDate = null;
                if (!String.IsNullOrEmpty(chartModel.StartDate))
                {
                    startDate = DateTime.ParseExact(chartModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!String.IsNullOrEmpty(chartModel.EndDate))
                {
                    endDate = DateTime.ParseExact(chartModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                //Tín NT :Phải xử lí đoạn này
                float totalgen = 0;
                float totalabs = 0;

                var lstExcute = chartService.Chart_ExportError(startDate, endDate, (byte)chartModel.WareHouseCode, (byte)chartModel.ArmNo, chartModel.Deviation);
                if (lstExcute.Rows.Count > 0)
                {
                    chartModel.CanDrawChart = true;
                    var DataList = new List<DataList>();
                    foreach (DataRow row in lstExcute.Rows)
                    {
                        var dataList = new DataList();
                        dataList.label = row["TimeOrder"].ToString();
                        dataList.y = Convert.ToInt32(row["V_Deviation"]);
                        DataList.Add(dataList);
                        totalgen += float.Parse(row["V_Deviation"].ToString());
                        totalabs += Math.Abs(float.Parse(row["V_Deviation"].ToString()));

                    }
                    chartModel.DataList = DataList;
                }

                var lstExcute2 = chartService.Chart_ExportError_PieChart(startDate, endDate, (byte)chartModel.WareHouseCode, (byte)chartModel.ArmNo, chartModel.Deviation);
                if (lstExcute2.Rows.Count > 0)
                {
                    chartModel.CanDrawChart = true;
                    var DataList_PieChart = new List<DataList2>();
                    foreach (DataRow row in lstExcute2.Rows)
                    {
                        var dataList = new DataList2();
                        dataList.indexLabel = row["V_Deviation"].ToString();
                        dataList.y = Convert.ToInt32(row["TimeCount"]);
                        //dataList.y = (float)row["CountTime"];
                        DataList_PieChart.Add(dataList);

                    }
                    chartModel.DataList2 = DataList_PieChart;
                }

                if (chartModel.DataList != null)
                {
                    chartModel.DiffMax = chartModel.DataList.Max(pl => pl.y);
                    chartModel.DiffMin = chartModel.DataList.Min(pl => pl.y);
                    chartModel.Avg = totalgen / chartModel.DataList.Count;
                    chartModel.AvgAbs = totalabs / chartModel.DataList.Count;
                }
                else
                {
                    chartModel.DiffMax = 0;
                    chartModel.DiffMin = 0;
                    chartModel.Avg = 0;
                    chartModel.AvgAbs = 0;
                }


                return View(chartModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }


        /// <summary>
        /// Vòng quay đo bể
        /// </summary>
        /// <param name="chartModel"></param>
        /// <returns></returns>
        public ActionResult TankAndWareHouse(ChartModel chartModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CHART_TANKANDWAREHOUSE);
            if (checkPermission)
            {


                chartModel.WareHouseList = warehouseService.GetAllWareHouse();
                if (chartModel.WareHouseList.Count() > 0)
                {
                    foreach (var it in chartModel.WareHouseList)
                    {
                        if (chartModel.WareHouseCode == null)
                        {
                            chartModel.WareHouseCode = it.WareHouseCode;
                            break;
                        }
                    }

                }
                //Lấy mặc định 1 nhóm
                //var lst = tankGroupService.GetAllTankGrp().FirstOrDefault();
                //if (lst != null)
                //{
                //    chartModel.TankGroupId = lst.TankGrpId;
                //}


                chartModel.TankGroupTempList = tankGroupService.GetAllTankGrp().Select(c => new TankGrp
                {
                    tankgrpid = c.TankGrpId,
                    tankgrpname = c.TanktGrpName,
                    warehousecode = c.WareHouseCode
                }).ToList();


                DateTime? startDate = null;
                DateTime? endDate = null;
                if (!String.IsNullOrEmpty(chartModel.StartDate))
                {
                    startDate = DateTime.ParseExact(chartModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!String.IsNullOrEmpty(chartModel.EndDate))
                {
                    endDate = DateTime.ParseExact(chartModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                //Tín NT :Phải xử lí đoạn này
                var lstExcute = chartService.Chart_TankAndWareHouse(startDate, endDate, (byte)chartModel.WareHouseCode, chartModel.TankGroupId);
                if (lstExcute.Rows.Count > 0)
                {
                    chartModel.CanDrawChart = true;
                    var DataList = new List<DataList>();
                    float exportVolume = 0;
                    float volumeMax = 0;
                    float rate;
                    foreach (DataRow row in lstExcute.Rows)
                    {
                        var dataList = new DataList();
                        dataList.label = row["TankName"].ToString();
                        dataList.y = float.Parse(Math.Round(Convert.ToDouble(row["Rate"]), 2).ToString());
                        exportVolume += float.Parse(Math.Round(Convert.ToDouble(row["ExportVolume"]), 2).ToString());
                        volumeMax += float.Parse(Math.Round(Convert.ToDouble(row["VolumeMax"]), 2).ToString());
                        DataList.Add(dataList);
                    }
                    rate = exportVolume / volumeMax;
                    var dataAllTank = new DataList();
                    dataAllTank.label = "Tổng bể";
                    dataAllTank.y = rate;
                    DataList.Add(dataAllTank);
                    chartModel.DataList = DataList;
                }

                return View(chartModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }


        /// <summary>
        /// Lịch sử bến xuất
        /// </summary>
        /// <param name="chartModel"></param>
        /// <returns></returns>
        public ActionResult HistoricalChartExport(ChartModel chartModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CHART_HISTORICALCHARTEXPORT);
            if (checkPermission)
            {


                chartModel.ProductList = productService.GetAllProductOrderByName();
                chartModel.TankTempList = tankService.GetAllTankOrderByName().Select(c => new TankTempModel
                {
                    ProductId = (int)c.ProductId,
                    TankId = c.TankId,
                    WareHouseCode = (byte)c.WareHouseCode,
                    TankName = c.TankName
                }).ToList();
                chartModel.WareHouseList = warehouseService.GetAllWareHouse();
                chartModel.ConfigArmTempList = configarmService.GetAllConfigArm().Select(c => new ConfigArmTempModel
                {
                    ArmName = c.ArmName,
                    ArmNo = c.ArmNo,
                    ConfigArmId = c.ConfigArmID,
                    WareHouseCode = c.WareHouseCode

                }).ToList();
                DateTime? startDate = null;
                DateTime? endDate = null;

                if (chartModel.WareHouseCode == null)
                {
                    if (Session[Constants.Session_WareHouse] == null)
                    {
                        if (chartModel.WareHouseList.Count() > 0)
                        {
                            foreach (var it in chartModel.WareHouseList)
                            {
                                chartModel.WareHouseCode = (byte)it.WareHouseCode;
                                break;
                            }
                        }

                    }
                    else
                    {
                        chartModel.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                    }
                }

                if (!String.IsNullOrEmpty(chartModel.StartDate))
                {
                    startDate = DateTime.ParseExact(chartModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                if (!String.IsNullOrEmpty(chartModel.EndDate))
                {
                    endDate = DateTime.ParseExact(chartModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                var logList = tankService.GetTankLogByTime(chartModel.WareHouseCode, chartModel.TankId, startDate, endDate);

                if (logList.Count() > 0)
                {
                    chartModel.CanDrawChart = true;
                    foreach (var log in logList)
                    {
                        if (chartModel.IsTotalLevel)
                        {
                            chartModel.TotalLevelData
                              .Add(new DataPoint(DateTimeUtil.GetJSTime(log.InsertDate), log.TotalLevel));
                        }
                        if (chartModel.IsProductVolume)
                        {
                            chartModel.ProductVolumeData
                            .Add(new DataPoint(DateTimeUtil.GetJSTime(log.InsertDate), (float?)log.ProductVolume));
                        }
                        if (chartModel.IsFlowRate)
                        {
                            chartModel.FlowRateData
                            .Add(new DataPoint(DateTimeUtil.GetJSTime(log.InsertDate), (float?)log.FlowRate));
                        }
                        if (chartModel.IsAvgTemperature)
                        {
                            chartModel.AvgTemperatureData
                            .Add(new DataPoint(DateTimeUtil.GetJSTime(log.InsertDate), log.AvgTemperature));
                        }
                        //if (chartModel.IsActualRatio)
                        //{
                        //    chartModel.ActualRatioData
                        //        .Add(new DataPoint(DateTimeUtil.GetJSTime(log.InsertDate), (float?)log.ActualRatio));
                        //}
                    }

                    chartModel.TotalLevelMin = chartModel.TotalLevelData.Min(pl => pl.y);
                    chartModel.TotalLevelMax = chartModel.TotalLevelData.Max(pl => pl.y);
                    chartModel.ProductVolumeMin = chartModel.ProductVolumeData.Min(pl => pl.y);
                    chartModel.ProductVolumeMax = chartModel.ProductVolumeData.Max(pl => pl.y);
                    chartModel.FlowRateMin = chartModel.FlowRateData.Min(pl => pl.y);
                    chartModel.FlowRateMax = chartModel.FlowRateData.Max(pl => pl.y);
                    chartModel.AvgTemperatureMin = chartModel.AvgTemperatureData.Min(pl => pl.y);
                    chartModel.AvgTemperatureMax = chartModel.AvgTemperatureData.Max(pl => pl.y);
                    //chartModel.ActualRatioMin = chartModel.ActualRatioData.Min(pl => pl.y);
                    //chartModel.ActualRatioMax = chartModel.ActualRatioData.Max(pl => pl.y);

                }

                return View(chartModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public JsonResult GetTankByWareHouse(byte warehouseCode)
        {
            try
            {
                //log.Info("Get SearchingData");
                var obj = tankService.GetAllTankByWareHouseCode(warehouseCode).ToList();
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //log.Error(ex);
                return null;
            }
        }
    }
}