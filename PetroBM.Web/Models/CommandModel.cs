using PagedList;
using PetroBM.Domain.Entities;
using System.Collections.Generic;
using System;
using PetroBM.Common.Util;

namespace PetroBM.Web.Models
{
    public class CommandModel
    {
        public CommandModel()
        {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,0,0,0).ToString(Constants.DATE_FORMAT);//   DateTime.Now.AddDays(Constants.DAYS_CALENDAR_OFFSET).ToString(Constants.DATE_FORMAT);
            EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).ToString(Constants.DATE_FORMAT);

            Invoice = new MInvoice();
            Command = new MCommand();
            ListProduct = new List<MProduct>();
            ListProductTemp = new List<ProductTemp>();
            ListSelectedField = new List<string>();
            ListTemProduct = new List<DataValue>();

        } 
        public MCommand Command { get; set; } 
        public MInvoice Invoice { get; set; }
        public string  WareHouseName { get; set; }
        public MCommandDetail CommandDetail { get; set; }
        public List<ProductTemp> ListProductTemp { get; set; } //Danh sách Mức giá
        public List<MProduct> ListProduct { get; set; }
        public List<MWareHouse> ListWareHouse { get; set; }        
        public List<WSystemSetting> ListInfo { get; set; }

        public List<DataValue> ListTemProduct { get; set; } 
        public List<DataValue> ListTemVehicle { get; set; }
        public IPagedList<MCommand> ListCommand{ get; set; }
        public IPagedList<MCommand> ListIECommand { get; set; }
        public List<MCommandDetail> ListCommandDetail { get; set; }
        public IPagedList<MCommandDetail> ListIECommandDetail { get; set; }
        public List<MSeal> ListSeal { get; set; }
		public List<MVehicle> ListVehicle { get; set; }
        public List<MCustomer> ListCustomer { get; set; }
        public List<Datum> LstDriver { get; set; }
        public List<string> ListSelectedField { get; set; }        
        public List<Datum> LstWareHouse { get; set; }
        public List<Datum> LstCustomer { get; set; }
        public List<Datum> LstVehicle { get; set; }
        public List<Datum> LstProduct { get; set; }
        public List<ListStatus> LstStatus { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string TimeOrder { get; set; }
        public long? WorkOrder { get; set; } //Lệnh
        public long? CertificateNumber { get; set; } //Số chứng từ
        public string CardSerial { get; set; }
        public string CardData { get; set; }
        public string VehicleNumber { get; set; }
        public byte? Status { get; set; }
		public int ExportMode { get; set; }
        public int? Flag { get; set; }
        public string TotalCommand { get; set; }
        public string CustomerName { get; set; }
        public string CertificateTime { get; set; }
    }
}