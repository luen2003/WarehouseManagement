using PagedList;
using PetroBM.Domain.Entities;
using System.Collections.Generic;
using System;
using PetroBM.Common.Util;
using System.Windows.Threading;

namespace PetroBM.Web.Models
{
    public class DispatchModel
    {
        public DispatchModel()
        {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,0,0,0).ToString(Constants.DATE_FORMAT);//   DateTime.Now.AddDays(Constants.DAYS_CALENDAR_OFFSET).ToString(Constants.DATE_FORMAT);
            EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).ToString(Constants.DATE_FORMAT);
            
            Dispatch = new MDispatch();
            ListProduct = new List<MProduct>();
            ListDepartment = new List<MDepartment>();
            ListProductTemp = new List<ProductTemp>();
            ListSelectedField = new List<string>();
            ListTemProduct = new List<DataValue>();
            ListDispatch = new List<MDispatch>();

        } 
        public MDispatch Dispatch { get; set; }  
        public string  WareHouseName { get; set; } 
        public List<ProductTemp> ListProductTemp { get; set; } //Danh sách Mức giá
        public List<MProduct> ListProduct { get; set; } 
        public List<MDispatch> ListDispatch { get; set; }
        public List<MDepartment> ListDepartment { get; set; }
        public List<MWareHouse> ListWareHouse { get; set; }        
        public List<WSystemSetting> ListInfo { get; set; }

        public List<DataValue> ListTemProduct { get; set; } 
        public List<DataValue> ListTemVehicle { get; set; }
        
        public IPagedList<MDispatch> ListIEDispatch { get; set; } 
        public List<MSeal> ListSeal { get; set; }
		public List<MVehicle> ListVehicle { get; set; }
        public List<MCustomer> ListCustomer { get; set; }
        public List<Datum> LstDriver { get; set; }
        public List<Datum> LstDepartment { get; set; }
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
        public string VehicleNumber { get; set; }
        public byte? Status { get; set; }  
        public string CustomerName { get; set; }
        public string CertificateTime { get; set; } 
        public string CertificateNumber { get; set; } 
        public string DriverName1 { get; set; }
        public string DriverName2 { get; set; }
        public string DstPickup1 { get; set; } 
        public string DstPickup2 { get; set; }
        public string DstReceive { get; set; }
        

    }
}