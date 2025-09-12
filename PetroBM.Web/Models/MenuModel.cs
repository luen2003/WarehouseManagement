using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class MenuModel
    {
        public MenuModel()
        {
            TankList = new HashSet<MTank>();
            TankGroupList = new HashSet<MTankGrp>();
            ProductList = new HashSet<MProduct>();
            WarehouseList = new HashSet<MWareHouse>();
            ListConfigArm = new HashSet<MConfigArm>();
            ConfigArmGroupList = new HashSet<MConfigArmGrp>();
            PermissionPositionWareHouseList = new List<MPermission>();
            PermissionGeneralList = new List<MPermission>();
        }

        public string ScreenName { get; set; }
        public IEnumerable<MTank> TankList { get; set; }
        public IEnumerable<MTankGrp> TankGroupList { get; set; }
        public IEnumerable<MProduct> ProductList { get; set; }
        public IEnumerable<MConfigArm> ListConfigArm { get; set; }
        public bool HasPermissionTank { get; set; }
        public bool HasPermissionTankGrp { get; set; }
        public bool HasPermissionProduct { get; set; }
        public bool HasPermissionImport { get; set; }
        public bool HasPermissionAlarm { get; set; }
        public bool HasPermissionConfig { get; set; }
        public bool HasPermissionWareHouse { get; set; } //Cấu hình cho kho
        public bool HasPermissionConfigGeneral { get; set; } // Cấu hình chung
        public bool HasPermissionConfigPositionWareHouse { get; set; } // Cấu hình tại kho
        public bool HasPermissionReport { get; set; }
        //Chi tiết từng quyền cho báo cáo
        public bool HasPermissionReport_IOTank { get; set; }
        public bool HasPermissionReport_BALANCETANK { get; set; }
        public bool HasPermissionReport_WAREHOUSECARD { get; set; }
        public bool HasPermissionReport_DATALOG { get; set; }
        public bool HasPermissionReport_TANKIMPORT { get; set; }
        public bool HasPermissionReport_TANKEXPORT { get; set; }
        public bool HasPermissionReport_DIFFERENCETANK { get; set; }
        public bool HasPermissionReport_HISTORICALDATATABLE { get; set; }
        public bool HasPermissionReport_HISTORYEXPORT { get; set; }
        public bool HasPermissionReport_EVENT { get; set; }
        public bool HasPermissionReport_WARNING { get; set; }
        public bool HasPermissionReport_ERROR { get; set; }
        public bool HasPermissionReport_FORCECASTTANK { get; set; }
        public bool HasPermissionReport_TANKMANUAL { get; set; }

        public bool HasPermissionChart { get; set; }
        //Chi tiết từng quyền cho đồ thị
        public bool HasPermissionChart_LIVECHART { get; set; }
        public bool HasPermissionChart_HISTORICALCHART { get; set; }
        public bool HasPermissionChart_LIVECHARTEXPORT { get; set; }
        public bool HasPermissionChart_HISTORICALCHARTEXPORT { get; set; }
        public bool HasPermissionChart_TANKANDWAREHOUSE { get; set; }
        public bool HasPermissionChart_EXPORTERROR { get; set; }
        public bool HasPermissionChart_DIFFEXPORTBYDAY { get; set; }
        public bool HasPermissionChart_USAGEPERFORMANCE { get; set; }
        public bool HasPermissionChart_PRODUCTSKPI { get; set; }
        public bool HasPermissionChart_TANKTURNOVER { get; set; }


        public bool HasPermissionManagement { get; set; }
        public bool HasPermissionReportGrp { get; set; }
        public IEnumerable<MWareHouse> WarehouseList { get; set; }
        public IEnumerable<MConfigArmGrp> ConfigArmGroupList { get; set; }
        public List<MPermission> PermissionPositionWareHouseList { get; set; }
        public List<MPermission> PermissionGeneralList { get; set; }
        public List<MWareHousePermission> WareHousePermissionList { get; set; }

        public int UserRole { get; set; }
    }
}