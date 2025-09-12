using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetroBM.Domain.Entities;
using PagedList;
using PetroBM.Common.Util;
using System.Data;

namespace PetroBM.Web.Models
{
    public class ChartModel
    {
        public ChartModel()
        {
            StartDate6h = DateTime.Now.AddHours(-6).ToString(Constants.DATE_FORMAT);
            StartDate = DateTime.Now.AddDays(Constants.DAYS_CALENDAR_OFFSET).ToString(Constants.DATE_FORMAT);
            EndDate = DateTime.Now.AddMinutes(1).ToString(Constants.DATE_FORMAT);

            TotalLevelColor = Constants.PRODUCT_LEVEL_COLOR;
            ProductVolumeColor = Constants.PRODUCT_VOLUME_COLOR;
            FlowRateColor = Constants.FLOW_RATE_COLOR;
            AvgTemperatureColor = Constants.AVG_TEMPERATURE_COLOR;
            ActualRatioColor = Constants.ACTUAL_RATIO_COLOR;
            FlowRateBaseColor = Constants.FlOW_RATE_BASE_COLOR;
            FlowRateEColor = Constants.FLOW_RATE_E_COLOR;

            IsTotalLevel = true;
            IsProductVolume = true;
            IsFlowRate = true;
            IsAvgTemperature = true;
            IsFlowRateBase = true;
            IsFlowRateE = true;
            IsActualRatio = true;
            IsDiffExport = true;        

            DurationList = new List<object>();
            TankList = new List<MTank>();
            TankGroupList = new List<MTankGrp>();
            TankTurnOverData = new List<object>();
            ProductsKPIData = new List<object>();
            TotalLevelData = new List<DataPoint>();
            ProductVolumeData = new List<DataPoint>();
            FlowRateData = new List<DataPoint>();
            AvgTemperatureData = new List<DataPoint>();
            WareHouseList = new List<MWareHouse>();
            //LiveDataArmList = new List<MLiveDataArm>();
            TankLiveList = new List<MTankLive>();
            TankTempList = new List<TankTempModel>();
            DataTable = new DataTable();
            DiffExportData = new List<DataList>();
        }

        public DataTable DataTable { get; set; }

        public IEnumerable<MWareHouse> WareHouseList { get; set; }
        public int Duration { get; set; }
        public IEnumerable<object> DurationList { get; set; }
        public IPagedList<MLiveDataArm> LiveDataArmList { get; set; }
        public IEnumerable<MLiveDataArm> LiveDataArmListByWareHouse{ get; set; }

        public byte ArmNo { get; set; }

        public double? Deviation { get; set; } // độ chênh lệch

        public string ProductCode  { get; set; }
        public string SerialNumber { get; set; }
        public int? TankId { get; set; }
        public byte? WareHouseCode { get; set; }
        public IEnumerable<MTank> TankList { get; set; }
        public List<TankTempModel> TankTempList { get; set; }
        public List<ConfigArmTempModel> ConfigArmTempList { get; set; }
        public IEnumerable<MProduct> ProductList { get; set; }
        public int TankGroupId { get; set; }
        public IEnumerable<MTankGrp> TankGroupList { get; set; }
        public List<TankGrp> TankGroupTempList { get; set; }
        public IPagedList<MTankLog> TankLogList { get; set; }
        public IEnumerable<MTankLive> TankLiveList { get; set; }
        public IEnumerable<MConfigArm> ConfigArmList { get; set; }
        public IEnumerable<MCommandDetail> CommandDetailList { get; set; }
        public byte? ConfigArmId { get; set; }

        public bool CanDrawChart { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartDate6h { get; set; }

        public List<DataList> DataList { get; set; }
        public List<DataList2> DataList2 { get; set; }
        public float? DiffMax { get; set; }
        public float? DiffMin { get; set; }

        public float? Avg { get; set; }
        public float? AvgAbs { get; set; }

        public List<DataList> DiffExportData { get; set; }
        public bool IsDiffExport { get; set; }
        public float? DiffExportMin;
        public float? DiffExportMax;

        public List<DataList> ErrorExportData { get; set; }
        public bool IsErrorExport { get; set; }
        public float? ErrorExportMin;
        public float? ErrorExportMax;

        public List<object> TankTurnOverData { get; set; }
        public List<object> ProductsKPIData { get; set; }
        public List<DataPoint> FlowRateBaseData { get; set; }
        public string FlowRateBaseColor { get; set; }
        public bool IsFlowRateBase { get; set; }
        public float? FlowRateBaseMin;
        public float? FlowRateBaseMax;

        public List<DataPoint> FlowRateEData { get; set; }
        public string FlowRateEColor { get; set; }
        public bool IsFlowRateE { get; set; }
        public float? FlowRateEMin;
        public float? FlowRateEMax;

        public List<DataPoint> FlowRateE5Data { get; set; }
        public string FlowRateE5Color { get; set; }
        public bool IsFlowRateE5 { get; set; }
        public float? FlowRateE5Min;
        public float? FlowRateE5Max;

        public List<DataPoint> MixingRatioData { get; set; }
        public string MixingRatioColor { get; set; }
        public bool IsMixingRatio { get; set; }
        public float? MixingRatioMin;
        public float? MixingRatioMax;

        public List<DataPoint> ActualRatioData { get; set; }
        public string ActualRatioColor { get; set; }
        public bool IsActualRatio { get; set; }
        public float? ActualRatioMin;
        public float? ActualRatioMax;


        public List<DataPoint> TotalLevelData { get; set; }
        public string TotalLevelColor { get; set; }
        public bool IsTotalLevel { get; set; }
        public float? TotalLevelMin;
        public float? TotalLevelMax;

        public List<DataPoint> ProductVolumeData { get; set; }
        public string ProductVolumeColor { get; set; }
        public bool IsProductVolume { get; set; }
        public float? ProductVolumeMin;
        public float? ProductVolumeMax;

        public List<DataPoint> FlowRateData { get; set; }
        public string FlowRateColor { get; set; }
        public bool IsFlowRate { get; set; }
        public float? FlowRateMin;
        public float? FlowRateMax;

        public List<DataPoint> AvgTemperatureData { get; set; }
        public string AvgTemperatureColor { get; set; }
        public bool IsAvgTemperature { get; set; }
        public float? AvgTemperatureMin;
        public float? AvgTemperatureMax;
    }
}