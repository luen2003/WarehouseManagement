using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetroBM.Domain.Entities;
using PagedList;
using System.ComponentModel.DataAnnotations;

namespace PetroBM.Web.Models
{
    public class ConfigurationModel
    {
        public ConfigurationModel()
        {
            SoundAlarmFile = new List<HttpPostedFileBase>();
            AlarmType = new List<MAlarmType>();

        }

        public WSystemSetting SystemConfig { get; set; }
        public List<HttpPostedFileBase> SoundAlarmFile { get; set; }
        public List<MAlarmType> AlarmType { get; set; }
        public List<string> SelectedFieldsConfig { get; set; }
        public enum FieldNames
        {
            [Display(Name = "Chiều cao nước")]
            WaterLevel = 2,
            [Display(Name = "Chiều cao hàng")]
            ProductLevel = 3,
            [Display(Name = "Chiều cao chung")]
            TotalLevel = 4,
            [Display(Name = "Thể tích nước")]
            WaterVolume = 5,
            [Display(Name = "Thể tích hàng (Vtt)")]
            ProductVolume = 6,
            [Display(Name = "Thể tích chung")]
            TotalVolume = 7,
            [Display(Name = "Thể tích trống")]
            TankEmpty = 8,
            [Display(Name = "Nhiệt độ trung bình")]
            AvgTemperature = 9,
            [Display(Name = "Nhiệt độ điểm 1")]
            Temperature1 = 10,
            [Display(Name = "Nhiệt độ điểm 2")]
            Temperature2 = 11,
            [Display(Name = "Nhiệt độ điểm 3")]
            Temperature3 = 12,
            [Display(Name = "Nhiệt độ điểm 4")]
            Temperature4 = 13,
            [Display(Name = "Nhiệt độ điểm 5")]
            Temperature5 = 14,
            [Display(Name = "Nhiệt độ điểm 6")]
            Temperature6 = 15,
            [Display(Name = "Nhiệt độ điểm 7")]
            Temperature7 = 16,
            [Display(Name = "Nhiệt độ điểm 8")]
            Temperature8 = 17,
            [Display(Name = "Nhiệt độ điểm 9")]
            Temperature9 = 18,
            [Display(Name = "Nhiệt độ điểm 10")]
            Temperature10 = 19,
            [Display(Name = "Level Rate")]
            LevelRate = 20,
            [Display(Name = "Flow Rate")]
            FlowRate = 21,
            [Display(Name = "Thể tích xuất được")]
            AvailableVolume = 22,
            [Display(Name = "Khối lượng")]
            Mass = 23,
            [Display(Name = "Mass Rate")]
            MassRate = 24,
            [Display(Name = "Thể tích hàng (V15)")]
            ProductVolume15 = 25,
            [Display(Name = "VCF")]
            VCF = 26,
            [Display(Name = "WCF")]
            WCF = 27
        }
        public List<string> SelectedConfigArmFieldsConfig { get; set; }
        public enum ConfigArmFieldNames
        {
            [Display(Name = "Thời gian")]
            TimeLog = 1,

            [Display(Name = "Kho")]
            WareHouseCode = 2,

            [Display(Name = "Họng số")]
            ArmNo = 3,

            [Display(Name = "Mã lệnh")]
            WorkOrder = 4,

            [Display(Name = "Ngăn")]
            CompartmentOrder = 5,

            [Display(Name = "Mã hàng hóa")]
            ProductCode = 6,

            [Display(Name = "Khách hàng")]
            CustomerName = 7,

            //[Display(Name = "Tên hàng hóa")]
            //ProductName = 7,

            [Display(Name = "Số phương tiện")]
            VehicleNumber = 8,

            [Display(Name = "Card data")]
            CardData = 9,

            [Display(Name = "Card serial")]
            CardSerial = 10,

            [Display(Name = "V đặt")]
            V_Preset = 11,

            [Display(Name = "Vtt")]
            V_Actual = 12,

            [Display(Name = "Vb")]
            V_Actual_Base = 13,

            [Display(Name = "Vtt_E")]
            V_Actual_E = 14,
            [Display(Name = "Lưu lượng")]
            Flowrate = 15,

            [Display(Name = "Lưu lượng Base")]
            Flowrate_Base = 16,

            [Display(Name = "Lưu lượng E")]
            Flowrate_E = 17,

            [Display(Name = "Nhiệt độ tb")]
            AvgTemperature = 18,

            [Display(Name = "Nhiệt độ tức thời")]
            CurrentTemperature = 19,

            [Display(Name = "Tỷ lệ trộn")]
            MixingRatio = 20,

            [Display(Name = "Tỷ lệ trộn tt")]
            ActualRatio = 21,

            [Display(Name = "Trạng thái")]
            Status = 22,

            [Display(Name = "ModeLog")]
            ModeLog = 23,

            [Display(Name = "Trạng thái dừng sự cố")]
            ESD = 24, 

        }
        public WSystemSetting HomeImageConfig { get; set; }
        public HttpPostedFileBase HomeImageFile { get; set; }
        public WSystemSetting LogoConfig { get; set; }
        public HttpPostedFileBase LogoFile { get; set; }
        public WSystemSetting CompanyConfig { get; set; }
        public WSystemSetting UnitConfig { get; set; }
		public WSystemSetting ExportMode { get; set; }


	}
}