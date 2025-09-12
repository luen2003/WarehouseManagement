using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetroBM.Domain.Entities;
using PagedList;
using PetroBM.Common.Util;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data;


namespace PetroBM.Web.Models
{
    public class ImportModel
    {
        public ImportModel()
        {
            StartDate = DateTime.Now.AddDays(Constants.DAYS_CALENDAR_OFFSET).ToString(Constants.DATE_FORMAT);
            EndDate = DateTime.Now.AddMinutes(1).ToString(Constants.DATE_FORMAT);
            //HandleDate = DateTime.Now.ToString(Constants.DATE_FORMAT);

            ImportInfo = new MImportInfo();
            ProductList = new List<MProduct>();
            TankImport = new MTankImport();
            TankImportTemp = new MTankImportTemp();

            TankImports = new List<MTankImport>();
            TankImportTemps = new List<MTankImportTemp>();
            ListClock = new List<MClock>();
            ListClockExport = new List<MClockExport>();

            TankTemps = new List<TankTempModel>();

            ManualInput = false;
        }



        public MImportInfo ImportInfo { get; set; }
        public IPagedList<MImportInfo> ImportInfoList { get; set; }
        public String StartDate { get; set; }
        public String EndDate { get; set; }
		public String CertificateTime { get; set; }		

		[Required(ErrorMessage ="*")]
        public String HandleDate { get; set; }
        
        public IEnumerable<MProduct> ProductList { get; set; }
        public int ProductId { get; set; }

        public IEnumerable<MWareHouse> WareHouseList { get; set; }
        public byte  WareHouseCode { get; set; }

        public int TankId { get; set; }

        public IEnumerable<MTank> TankList { get; set; }

        public MTankImport TankImport { get; set; }
        public MTankImportTemp TankImportTemp { get; set; }

        public IList<MTankImport> TankImports { get; set; }

        public IList<MTankImportTemp> TankImportTemps { get; set; }

        public IList<MClock> ListClock { get; set; }

        public IList<MClockExport> ListClockExport { get; set; }

        public IList<TankTempModel> TankTemps { get; set; }

        public IList<MConfigArm> ListConfigArm { get; set; }

        public IList<MExportArmImport> ListExportArmImport { get; set; }

        //public List<SelectListItem> TankList { get; set; }

        //public List<MTank> TankList { get; set; }

        public int[] ListTankId { get; set; }
        public string ListTankName { get; set; }

        public MTankLive TankLive { get; set; }

        public List<byte> ListArmNo { get; set; }
        public string ListArmNoName { get; set; }

        public DataTable VttAndV15 { get; set; }

        public DataTable TankImportData { get; set; }
        public DataTable ClockExportData { get; set; }
        public DataTable TankImportInfoData { get; set; }
        public DataTable ExportArmImportData { get; set; }

        [Required(ErrorMessage ="*")]
        [Display(Name = "Số lượng hàng xuất Vtt")]
        [Range(0, Double.MaxValue, ErrorMessage = "Số lượng hàng xuất Vtt >= 0")]
        public double ExportVtt { get; set; }
        [Required(ErrorMessage ="*")]
        [Display(Name = "Số lượng hàng xuất V15")]
        [Range(0, Double.MaxValue,ErrorMessage = "Số lượng hàng xuất V15 >= 0")]
        public double ExportV15 { get; set; }

        public bool ManualInput { get; set; } //check manual input here
       
        public string ManualMessage { get; set; }// import message

        public MTankLog TankLogData { get; set; }

    }
}