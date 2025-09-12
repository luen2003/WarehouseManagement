using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class Invoice
    {
       public Invoice() { }
       public  Invoice(string No, string ProductName, string ListVolume, string CalcUnit, float Value, string LevelAmount, string Status,string ListV,int Discount)
        {
            this.No = No;
            this.ProductName = ProductName;
            this.Value = Value;
            this.CalcUnit = CalcUnit;
            this.ListVolume = ListVolume;
            this.LevelAmount = LevelAmount;
            this.Status = Status;
            this.ListV = ListV;
            this.Discount = Discount;
        }

        public Invoice(string No, string ProductName, string ListVolume, string CalcUnit, float Value, string LevelAmount, string Status)
        {
            this.No = No;
            this.ProductName = ProductName;
            this.Value = Value;
            this.CalcUnit = CalcUnit;
            this.ListVolume = ListVolume;
            this.LevelAmount = LevelAmount;
            this.Status = Status;
        }

        public Invoice(string No, string ProductName, string ListVolume, string CalcUnit, float Value, string LevelAmount, string Status, double PriceUnit,bool EnvironmentTax,int Discount,double Vcf,double Density, double Temp)
        {
            this.No = No;
            this.ProductName = ProductName;
            this.Value = Value;
            this.CalcUnit = CalcUnit;
            this.ListVolume = ListVolume;
            this.LevelAmount = LevelAmount;
            this.Status = Status;
            this.PriceUnit = PriceUnit;
            this.EnvironmentTax = EnvironmentTax;
            this.Discount = Discount;
            this.Vcf = Vcf;
            this.Temp = Temp;
            this.Density = Density;
        }

        public string No { get; set; }
		public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ListVolume { get; set; } //Danh sách các ngăn: mỗi 1 ký tự là 1 ngăn tương ứng với ký tự đó
        public string ListV { get; set; } //Thể tích tương ứng với ngăn phân biệt bởi đấu ","
        public string CalcUnit { get; set; }
        public float Value { get; set; }
        public string LevelAmount { get; set; } //Mức giá
        public double PriceUnit { get; set; } //Giá ăn theo mức giá
        public string  Status { get; set; } //0 : Chưa tách hóa đơn, 1: Đã tách hóa đơn
        public int Discount { get; set; } //Chiết khấu
        public bool EnvironmentTax { get; set; } // Thuế môi trường
        public double Vcf { get; set; } // VCF trung bình
        public double Density { get; set; } //Density trung bình
        public double Temp { get; set; } // Nhiệt độ trung bình

    }
}