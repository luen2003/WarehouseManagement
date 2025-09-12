using PagedList;
using PetroBM.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System;
using PetroBM.Common.Util;

namespace PetroBM.Web.Models
{
    public class InvoiceModel
    {
        public InvoiceModel()
        {
            Invoice = new MInvoice();
            ListSelectedField = new List<string>();
            OutTime = DateTime.Now.ToString(Constants.DATE_FORMAT);
            StartDate = DateTime.Now.AddDays(Constants.DAYS_CALENDAR_OFFSET).ToString(Constants.DATE_FORMAT);
            EndDate = DateTime.Now.AddMinutes(1).ToString(Constants.DATE_FORMAT);
        }

        public MInvoice Invoice { get; set; }
        public string WareHouseName { get; set; }
        public byte? WareHouse { get; set; }
        public IPagedList<MInvoice> ListPLInvoice { get; set; }
        public IEnumerable<MInvoice> ListEnumInvoice { get; set; }
        public List<Invoice> ListInvoice { get; set; }
        public List<MInvoiceDetail> ListInvoiceDetail { get; set; }
        public List<MCommandDetail> ListCommandDetail { get; set; }
        public List<MDriver> ListDriver { get; set; }
		public List<MCommand> ListCommand { get; set; }
        public MCommand Command { get; set; }
        public List<MSeal> ListSeal { get; set; }
        public List<string> ListSelectedField { get; set; }
        public List<DataValue> ListPrice { get; set; } //Danh sách Mức giá
        public List<MProduct> ListProduct { get; set; } //Danh sách Mức giá
        public List<ProductTemp> ListProductTemp { get; set; } //Danh sách Mức giá
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
		public long? WorkOrder { get; set; }
		public string VerhicleNumber { get; set; }
        public long? CardSerial { get; set; }
		public string EnvironmentTax { get; set; }
        public string PriceLevel { get; set; }
        public double? AvgVcf { get; set; }

        public double? AvgDensity { get; set; }

        public double? AvgTemp { get; set; }

		public int ExportMode { get; set; }
	}
}