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
	public class ReportModel
	{
		public ReportModel()
		{
			Month = DateTime.Now.ToString(Constants.DATE_MONTH_FORMAT);
			StartDate = DateTime.Now.ToString(Constants.DATE_FORMAT_NOTTIME);
			EndDate = DateTime.Now.AddMinutes(1).ToString(Constants.DATE_FORMAT);

			TankList = new List<MTank>();
			TankGroupList = new List<MTankGrp>();
			AlarmTypeList = new List<MAlarmType>();
			EventTypeList = new List<MEventType>();
			ListProduct = new List<MProduct>();
			ListWareHouse = new List<MWareHouse>();
			ListConfigArm = new List<MConfigArm>();
			ListCommand = new List<MCommand>();
			ListBalanceTank = new List<BalanceModel>();
            ListCommandDetail = new List<MCommandDetail>();
		}

		public IPagedList<MAlarm> AlarmList { get; set; }
		public IPagedList<MEvent> EventList { get; set; }

		public string Month { get; set; }
		public string StartDate { get; set; }
		public string EndDate { get; set; }
		public string FileType { get; set; }
		public byte? ArmNo { get; set; }
		public decimal? WorkOrder { get; set; }
		public string Vehicle { get; set; }
        public string Customer { get; set; }
        public int? Deviation { get; set; }
		public string CustomerName { get; set; }
        public string CustomerGroup { get; set; }
        public DataTable DataTable { get; set; }
		public DataTable DataTable2 { get; set; }
		public byte? TypeExport { get; set; }
		public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int? CardSerial { get; set; }
		public int? TankId { get; set; }
		public string TankName { get; set; }
		public string TypeName { get; set; }
		public IEnumerable<MTank> TankList { get; set; }

		public int? AlarmTypeId { get; set; }
		public int? TypeId { get; set; }
		public byte? WareHouseCode { get; set; }
		public string WareHouseName { get; set; }
		public IEnumerable<BalanceModel> ListBalanceTank { get; set; }
		public IEnumerable<MAlarmType> AlarmTypeList { get; set; }
		public IEnumerable<MConfigArm> ListConfigArm { get; set; }
		public IEnumerable<MTankManual> ListTankManual { get; set; }
		public MTankManual TankManual { get; set; }
		public IEnumerable<MProduct> ListProduct { get; set; }
		public IEnumerable<MWareHouse> ListWareHouse { get; set; } 
        public List<Datum> ListVehicleFillter { get; set; }
        public List<Datum> ListCustomer { get; set; } 
        public List<Datum> ListCustomerGroup { get; set; }
        public IEnumerable<MCommand> ListCommand { get; set; }
        public IEnumerable<MCommandDetail> ListCommandDetail { get; set; }
		public int? ConfigArmID { get; set; }
		public int? CommandID { get; set; }
		public int? ProductId { get; set; }
		public int? TankGroupId { get; set; }
		public string TankGroupName { get; set; }
		public int? Id { get; set; }
		public IEnumerable<MTankGrp> TankGroupList { get; set; }
		public IPagedList<MImportInfo> ImportInfoList { get; set; }
		public MImportInfo ImportInfo { get; set; }
		public int? ImportId { get; set; }

		public DataTable TankImportData { get; set; }
		public DataTable ClockExportData { get; set; }
		public DataTable TankImportInfoData { get; set; }
		public DataTable ExportArmImportData { get; set; }
        public DataTable ListTankLog { get; set; }


		public string User { get; set; }
		public int? EventTypeId { get; set; }
		public IEnumerable<MEventType> EventTypeList { get; set; }
		public double DeviationAverage { get; set; } //% sai số TB
		public double PercentLevel { get; set; } // % sai số theo mức

		public TWarehouseCard WarehouseCard { get; set; }

        public List<MVehicle> ListVehicle { get; set; }
        public List<MCommand> ListCommandOfSearch { get; set; }
        public List<MCommandDetail> ListCommandDetailOfSearch { get; set; }

        public DataTable BangKeChiTietTheoNgan { get; set; }
        public DataTable BangKeChiTietTheoChungTu { get; set; }
        public DataTable BangKeTHTheoHong { get; set; }
        public DataTable BangKeTHTheoHong_Total { get; set; }
        public DataTable BangKeTHTheoPhuongTien { get; set; } 
        public DataTable BangKeTHTheoPhuongTien_Total { get; set; } 
        public DataTable BangKeTHTheoNhomKhach { get; set; }
        public DataTable BangKeTHTheoNhomKhach_Total { get; set; } 
        public DataTable BangKeTHTheoNhomKhach2 { get; set; }
        public DataTable BangKeTHTheoNhomKhach2_Total { get; set; } 
        public DataTable BangKeCTRaVaoKho { get; set; }
        public DataTable BangKeCTRaVaoKho_Total { get; set; }
        public DataTable BangKeChiTiet { get; set; }
        public DataTable BangKeChiTiet_ToTal { get; set; }

    }
}