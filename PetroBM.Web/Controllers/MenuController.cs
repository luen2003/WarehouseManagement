using PetroBM.Common.Util;
using PetroBM.Web.Models;
using PetroBM.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetroBM.Web.Controllers
{
    public class MenuController : BaseController
    {
        private readonly IUserService userService;
        private readonly ITankService tankService;
        private readonly ITankGroupService tankGroupService;
        private readonly IProductService productService;
        private readonly IConfigArmService configarmService;
        private readonly MenuModel menuModel;
        private readonly IWareHouseService warehouseService;
        private readonly IConfigArmGrpService configarmGrpService;

        public MenuController(IUserService userService, ITankService tankService, ITankGroupService tankGroupService,
            IProductService productService, MenuModel menuModel, IWareHouseService warehouseService, IConfigArmService configarmService, IConfigArmGrpService configarmGrpService)
        {
            this.userService = userService;
            this.tankService = tankService;
            this.tankGroupService = tankGroupService;
            this.productService = productService;
            this.menuModel = menuModel;
            this.warehouseService = warehouseService;
            this.configarmService = configarmService;
            this.configarmGrpService = configarmGrpService;
        }

        [ChildActionOnly]
        public PartialViewResult Menu()
        {
            menuModel.TankList = tankService.GetAllTankOrderByName();
            menuModel.TankGroupList = tankGroupService.GetAllTankGrpOrderByName();
            menuModel.ProductList = productService.GetAllProductOrderByName();
            menuModel.ListConfigArm = configarmService.GetAllConfigArm();
            menuModel.ConfigArmGroupList = configarmGrpService.GetAllConfigArmGrp();

            String userName = HttpContext.User.Identity.Name;
            menuModel.WarehouseList = warehouseService.GetWareHouse_ByUserName_Menu(userName); ;// Lấy kho theo phân quyền
            menuModel.HasPermissionTank = userService.CheckPermission(userName, Constants.PERMISSION_TANK, Constants.FLAG_OFF);
            menuModel.HasPermissionTankGrp = userService.CheckPermission(userName, Constants.PERMISSION_TANK_GRP, Constants.FLAG_OFF);
            menuModel.HasPermissionProduct = userService.CheckPermission(userName, Constants.PERMISSION_PRODUCT, Constants.FLAG_OFF);
            menuModel.HasPermissionImport = userService.CheckPermission(userName, Constants.PERMISSION_IMPORT, Constants.FLAG_OFF);
            menuModel.HasPermissionAlarm = userService.CheckPermission(userName, Constants.PERMISSION_ALARM, Constants.FLAG_OFF);
            menuModel.HasPermissionConfig = userService.CheckPermission(userName, Constants.PERMISSION_CONFIGURATION, Constants.FLAG_OFF);

            menuModel.HasPermissionConfigGeneral = userService.CheckPermission_General(userName, Constants.PERMISSION_CONFIGGENERAL, Constants.FLAG_OFF);

            if(menuModel.HasPermissionConfigGeneral)
{
                menuModel.PermissionGeneralList = userService.GetListPermission_ByUserName(userName, Constants.PERMISSION_CONFIGGENERAL, Constants.PERMISSION_LISTLOCATION);
            }

            menuModel.HasPermissionConfigPositionWareHouse = userService.CheckPermission_General(userName, Constants.PERMISSION_CONFIGPOSITIONWAREHOUSE, Constants.FLAG_OFF);

            if (menuModel.HasPermissionConfigPositionWareHouse)
            {
                menuModel.PermissionPositionWareHouseList = userService.GetListPermission_ByUserName(userName, Constants.PERMISSION_CONFIGPOSITIONTANK, Constants.PERMISSION_CONFIGPOSITIONEXPORTMODE);
            }
            //Các quyền cho kho
            menuModel.WareHousePermissionList = userService.GetListWareHousePermission_ByUserName(userName, Constants.PERMISSION_WAREHOUSE_MANUAL, Constants.PERMISSION_WAREHOUSE_REGISTERDISPATCH);
            menuModel.UserRole = userService.GetJobTitlesByUserName(userName);
            menuModel.HasPermissionConfigGeneral = userService.CheckPermission_General(userName, Constants.PERMISSION_CONFIGGENERAL, Constants.FLAG_OFF);
            menuModel.HasPermissionReport = userService.CheckPermission_ManagerReportChart(userName, Constants.PERMISSION_REPORT, Constants.FLAG_OFF);
            if (menuModel.HasPermissionReport)
            {
                menuModel.HasPermissionReport_BALANCETANK = userService.CheckPermission_General(userName, Constants.PERMISSION_REPORT_BALANCETANK, Constants.FLAG_OFF);
                menuModel.HasPermissionReport_WAREHOUSECARD = userService.CheckPermission_General(userName, Constants.PERMISSION_REPORT_WAREHOUSECARD, Constants.FLAG_OFF);
                menuModel.HasPermissionReport_DATALOG = userService.CheckPermission_General(userName, Constants.PERMISSION_REPORT_DATALOG, Constants.FLAG_OFF);
                menuModel.HasPermissionReport_DIFFERENCETANK = userService.CheckPermission_General(userName, Constants.PERMISSION_REPORT_DIFFERENCETANK, Constants.FLAG_OFF);
                menuModel.HasPermissionReport_ERROR = userService.CheckPermission_General(userName, Constants.PERMISSION_REPORT_ERROR, Constants.FLAG_OFF);
                menuModel.HasPermissionReport_EVENT = userService.CheckPermission_General(userName, Constants.PERMISSION_REPORT_EVENT, Constants.FLAG_OFF);
                menuModel.HasPermissionReport_FORCECASTTANK = userService.CheckPermission_General(userName, Constants.PERMISSION_REPORT_FORCECASTTANK, Constants.FLAG_OFF);
                menuModel.HasPermissionReport_HISTORICALDATATABLE = userService.CheckPermission_General(userName, Constants.PERMISSION_REPORT_HISTORICALDATATABLE, Constants.FLAG_OFF);
                menuModel.HasPermissionReport_HISTORYEXPORT = userService.CheckPermission_General(userName, Constants.PERMISSION_REPORT_HISTORYEXPORT, Constants.FLAG_OFF);
                menuModel.HasPermissionReport_IOTank = userService.CheckPermission_General(userName, Constants.PERMISSION_REPORT_IOTank, Constants.FLAG_OFF);
                menuModel.HasPermissionReport_TANKEXPORT = userService.CheckPermission_General(userName, Constants.PERMISSION_REPORT_TANKEXPORT, Constants.FLAG_OFF);
                menuModel.HasPermissionReport_TANKIMPORT = userService.CheckPermission_General(userName, Constants.PERMISSION_REPORT_TANKIMPORT, Constants.FLAG_OFF);
                menuModel.HasPermissionReport_TANKMANUAL = userService.CheckPermission_General(userName, Constants.PERMISSION_REPORT_TANKMANUAL, Constants.FLAG_OFF);
                menuModel.HasPermissionReport_WARNING = userService.CheckPermission_General(userName, Constants.PERMISSION_REPORT_WARNING, Constants.FLAG_OFF);
                menuModel.HasPermissionReportGrp = userService.CheckPermission_General(userName, Constants.PERMISSION_REPORT_GRP, Constants.FLAG_OFF);
            }
            menuModel.HasPermissionChart = userService.CheckPermission_ManagerReportChart(userName, Constants.PERMISSION_CHART, Constants.FLAG_OFF);
            if (menuModel.HasPermissionChart)
            {
                menuModel.HasPermissionChart_DIFFEXPORTBYDAY = userService.CheckPermission_General(userName, Constants.PERMISSION_CHART_DIFFEXPORTBYDAY, Constants.FLAG_OFF);
                menuModel.HasPermissionChart_EXPORTERROR = userService.CheckPermission_General(userName, Constants.PERMISSION_CHART_EXPORTERROR, Constants.FLAG_OFF);
                menuModel.HasPermissionChart_HISTORICALCHARTEXPORT = userService.CheckPermission_General(userName, Constants.PERMISSION_CHART_HISTORICALCHARTEXPORT, Constants.FLAG_OFF);
                menuModel.HasPermissionChart_HISTORICALCHART = userService.CheckPermission_General(userName, Constants.PERMISSION_CHART_HISTORICALCHART, Constants.FLAG_OFF);
                menuModel.HasPermissionChart_LIVECHARTEXPORT = userService.CheckPermission_General(userName, Constants.PERMISSION_CHART_LIVECHARTEXPORT, Constants.FLAG_OFF);
                menuModel.HasPermissionChart_LIVECHART = userService.CheckPermission_General(userName, Constants.PERMISSION_CHART_LIVECHART, Constants.FLAG_OFF);
                menuModel.HasPermissionChart_PRODUCTSKPI = userService.CheckPermission_General(userName, Constants.PERMISSION_CHART_PRODUCTSKPI, Constants.FLAG_OFF);
                menuModel.HasPermissionChart_TANKANDWAREHOUSE = userService.CheckPermission_General(userName, Constants.PERMISSION_CHART_TANKANDWAREHOUSE, Constants.FLAG_OFF);
                menuModel.HasPermissionChart_USAGEPERFORMANCE = userService.CheckPermission_General(userName, Constants.PERMISSION_CHART_USAGEPERFORMANCE, Constants.FLAG_OFF);
                menuModel.HasPermissionChart_TANKTURNOVER = userService.CheckPermission_General(userName, Constants.PERMISSION_CHART_TANKTURNOVER, Constants.FLAG_OFF);
            }

            menuModel.HasPermissionManagement = userService.CheckPermission_ManagerReportChart(userName, Constants.PERMISSION_MANAGEMENT, Constants.FLAG_OFF);

            return PartialView(menuModel);
        }

        [ChildActionOnly]
        public PartialViewResult TopNavBar(string screenName)
        {
            menuModel.ScreenName = screenName;
            return PartialView(menuModel);
        }

        [HttpPost]
        public ActionResult CheckDataServerRunning()
        {
            return Content(tankService.CheckDataServerRunning().ToString());
        }

        public ActionResult WarehouseSelect(int warehousecode)
        {
            Session.Add(Constants.Session_WareHouse, warehousecode);
            Session[Constants.Session_WareHouseName] = warehouseService.GetAllWareHouse().Where(x => x.WareHouseCode == warehousecode).First().WareHouseName;

            return null;
        }
    }
}