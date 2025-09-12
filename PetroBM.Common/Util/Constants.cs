using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Common.Util
{
    public class Constants
    {
        public const int PAGE_SIZE = 15;
        
        public const int ALARM_NUMBER = 6;
        public const int EVENT_NUMBER = 6;
        
        public const int CHECK_DATA_SERVER_TIME = 30;   // second

        public const string SPLIT_CHAR = ",";

        public const int HOURS_LIVE_CHART = 2;
        public const int MINUTE_DATALOG_OFFSET = -5;
        public const int DAYS_ALARM_OFFSET = -3;
        public const int DAYS_CALENDAR_OFFSET = -1;

        public const string DATE_FORMAT_NEW = "dd/MM/yyyy";
		public const string DATE_MONTH_FORMAT = "MM/yyyy";

        public const string DATE_FORMAT_NOTTIME = "dd/MM/yyyy 00:00";
        public const string DATE_FORMAT = "dd/MM/yyyy HH:mm";
        public const string DATE_FORMAT_STRING = "{0:dd/MM/yyyy HH:mm:ss}";

        public const string PASS_FORMAT = "MD5";

        public const string PRODUCT_LEVEL_COLOR = "#CC9900";
        public const string PRODUCT_VOLUME_COLOR = "#339A03";
        public const string FLOW_RATE_COLOR = "#009AFE";
        public const string AVG_TEMPERATURE_COLOR = "#FF0063";

        public const string FlOW_RATE_BASE_COLOR = "#CC9900";
        public const string FLOW_RATE_E_COLOR = "#339A03";
        public const string ACTUAL_RATIO_COLOR = "#E82C04";

        public const string SOUND_ALARM_PATH = "/Content/Sound";
        public const string FILE_PATH = "/Content/file";
        public const string IMG_PATH = "/Content/images/";

        public const bool FLAG_OFF = false;
        public const bool FLAG_ON = true;

        public const string NULL = "null";

        public const int VERSION_START = 1;

        public const int TEC_CC_ID = 9999;
        public const float TEC_CC_CONSTANT = (float)1.88;

        public const float MIN_DENSITY = (float)0.6;

		// Export Mode
		public const int EXPORT_MODE_CARD = 1;
		public const int EXPORT_MODE_WORKORDER = 2;

		// CommandFlag
		public const int COMMAND_FLAG_INIT = 0;
		public const int COMMAND_FLAG_OLD = 1;
		public const int COMMAND_FLAG_NEW = 2;

		public const int ADMIN_GROUP_ID = 1;
        public const int PERMISSION_TANK = 1;
        public const int PERMISSION_TANK_GRP = 2;
        public const int PERMISSION_PRODUCT = 3;
        public const int PERMISSION_IMPORT = 4;
        public const int PERMISSION_ALARM = 5;
        public const int PERMISSION_CONFIGURATION = 8;

        // KHO
        public const int PERMISSION_WAREHOUSE_MANUAL = 600;
        public const int PERMISSION_WAREHOUSE_REGISTERCOMMAND = 601; 
        public const int PERMISSION_WAREHOUSE_REGISTERSEAL = 602;
        public const int PERMISSION_WAREHOUSE_REGISTERINVOICE = 603;
        public const int PERMISSION_WAREHOUSE_REGISTERINOUTMANUAL = 604;
		public const int PERMISSION_WAREHOUSE_APPROVECOMMAND = 605;
		public const int PERMISSION_WAREHOUSE_CANCELCOMMAND = 606;
		public const int PERMISSION_WAREHOUSE_TANK = 620;
        public const int PERMISSION_WAREHOUSE_TANKGROUP = 630;
        public const int PERMISSION_WAREHOUSE_PRODUCT = 640;
        public const int PERMISSION_WAREHOUSE_CONFIGARM = 650;
        public const int PERMISSION_WAREHOUSE_CONFIGARMGROUP = 660;
        public const int PERMISSION_WAREHOUSE_IMPORTPRODUCT = 670;
        public const int PERMISSION_WAREHOUSE_REPORT1 = 680;
        public const int PERMISSION_WAREHOUSE_REPORT2 = 681;
        public const int PERMISSION_WAREHOUSE_REGISTERDISPATCH = 701;
        // BÁO CÁO 300
        public const int PERMISSION_REPORT = 300;//public const int PERMISSION_REPORT = 6;
        public const int PERMISSION_REPORT_IOTank = 301;//nhập, xuất tồn, trống bể
        public const int PERMISSION_REPORT_BALANCETANK = 302;//cân đối nhập, xuất tồn
        public const int PERMISSION_REPORT_DATALOG = 303;//tồn kho
        public const int PERMISSION_REPORT_TANKIMPORT = 304;//nhập hàng
        public const int PERMISSION_REPORT_TANKEXPORT = 305;//xuất hàng
        public const int PERMISSION_REPORT_DIFFERENCETANK = 306;//Sai số mẻ, chênh lệch lượng hàng
        public const int PERMISSION_REPORT_HISTORICALDATATABLE = 307;//Dữ liệu lịch sử đo bể
        public const int PERMISSION_REPORT_HISTORYEXPORT = 308;//Dữ liệu lịch sử bến xuất
        public const int PERMISSION_REPORT_EVENT = 309;//Sự kiện
        public const int PERMISSION_REPORT_WARNING = 310;//Cảnh báo
        public const int PERMISSION_REPORT_ERROR = 311;//Lỗi
        public const int PERMISSION_REPORT_FORCECASTTANK = 312;//Dự báo điều hàng
        public const int PERMISSION_REPORT_TANKMANUAL = 313;//Đo tay
		public const int PERMISSION_REPORT_WAREHOUSECARD = 314;//Thẻ kho
        public const int PERMISSION_REPORT_GRP = 315;//Nhóm báo cáo chi tiết, tổng hợp

        // ĐỒ THỊ 400
        public const int PERMISSION_CHART = 400;//public const int PERMISSION_CHART = 7;
        public const int PERMISSION_CHART_LIVECHART = 401;//Thời gian thực đo bể
        public const int PERMISSION_CHART_HISTORICALCHART = 402;//Lịch sử đo bể
        public const int PERMISSION_CHART_LIVECHARTEXPORT = 403;//Thời gian thực bến xuất
        public const int PERMISSION_CHART_HISTORICALCHARTEXPORT = 404;//Lịch sử bến xuất
        public const int PERMISSION_CHART_TANKTURNOVER = 405;//Vòng quay đo bể
        public const int PERMISSION_CHART_EXPORTERROR = 406;//Sai số mẻ xuất
        public const int PERMISSION_CHART_DIFFEXPORTBYDAY = 407;//Chênh lệch lượng xuất
        public const int PERMISSION_CHART_USAGEPERFORMANCE = 408;//Hiệu xuất sử dụng
        public const int PERMISSION_CHART_PRODUCTSKPI = 409;//Tỷ lệ hàng hóa
        public const int PERMISSION_CHART_TANKANDWAREHOUSE = 410;//Tỷ lệ hàng hóa

        // QUẢN TRỊ HỆ THỐNG
        public const int PERMISSION_MANAGEMENT = 100;// QUẢN TRỊ HỆ THỐNG  //public const int PERMISSION_MANAGEMENT = 9;
        //CẤU HÌNH CHUNG
        public const int PERMISSION_CONFIGGENERAL = 200;// CẤU HÌNH CHUNG
        public const int PERMISSION_CONFIGCOMPANY = 201;// THÔNG TIN CHUNG
        public const int PERMISSION_LISTWAREHOUSE = 202;// DANH MỤC KHO
        public const int PERMISSION_LISTVERHICLE = 203;// DANH MỤC PHƯƠNG TIỆN
        public const int PERMISSION_LISTDRIVER = 204;// DANH MỤC LÁI XE
        public const int PERMISSION_LISTPRODUCT = 205;// DANH MỤC HÀNG HÓA
        public const int PERMISSION_LISTPRICE = 207;// DANH MỤC GIÁ
        public const int PERMISSION_LISTCUSTOMER = 206;//DANH MỤC KHÁCH HÀNG
        public const int PERMISSION_LISTCUSTOMERGROUP = 208;//DANH MỤC NHÓM KHÁCH HÀNG
        public const int PERMISSION_LISTSHIP = 209;// DANH MỤC PHƯƠNG TIỆN THỦY
        public const int PERMISSION_LISTLOCATION = 210;// DANH MỤC ĐỊA ĐIỂM
        //CẤU HÌNH TẠI KHO
        public const int PERMISSION_CONFIGPOSITIONWAREHOUSE = 500;// CẤU HÌNH TẠI KHO
        public const int PERMISSION_CONFIGPOSITIONTANK = 501;// BỂ
        public const int PERMISSION_CONFIGPOSITIONTANKGROUP = 502;// NHÓM BỂ
        public const int PERMISSION_CONFIGPOSITIONTANKDISPLAY = 503;// CHỌN TRƯỜNG HIỂN THỊ BỂ
        public const int PERMISSION_CONFIGPOSITIONCONFIGARM = 504;// HỌNG XUẤT
        public const int PERMISSION_CONFIGPOSITIONCONFIGARMGROUP = 505;// NHÓM HỌNG XUẤT
        public const int PERMISSION_CONFIGPOSITIONCONFIGARMDISPLAY = 506;// CHỌN TRƯỜNG HIỂN THỊ HỌNG XUẤT
        public const int PERMISSION_CONFIGPOSITIONCARD = 507;// CHỌN TRƯỜNG HIỂN THỊ HỌNG XUẤT
        public const int PERMISSION_CONFIGPOSITIONWARMINGEVENT = 508;// ÂM THANH CẢNH BÁO SỰ KIỆN
		public const int PERMISSION_CONFIGPOSITIONEXPORTMODE = 509;// ÂM THANH CẢNH BÁO SỰ KIỆN


		public const int ALARM_TYPE_ERROR = 1;
        public const int ALARM_TYPE_ALARM_LEVEL = 2;
        public const int ALARM_TYPE_ALARM_TEMP = 3;
        public const int ALARM_TYPE_ALARM_FLOW = 4;

        public const int EVENT_TYPE_AUTHENTICATION = 1;
        public const int EVENT_TYPE_REPORT = 2;
        public const int EVENT_TYPE_CONFIGURATION = 3;
        public const int EVENT_TYPE_MANAGEMENT = 4;
        public const int EVENT_TYPE_IMPORT = 5;

        public const string EVENT_LOGIN = "Login";
        public const string EVENT_LOGOUT = "Logout";

        public const string EVENT_IMPORT_CREATE = "Tạo mới nhập hàng";
        public const string EVENT_IMPORT_UPDATE = "Sửa thông tin nhập hàng";
        public const string EVENT_IMPORT_DELETE = "Xóa thông tin nhập hàng";
        public const string EVENT_IMPORT_START_HANDLE = "Chốt đầu";
        public const string EVENT_IMPORT_END_HANDLE = "Chốt cuối";
        public const string EVENT_IMPORT_CLOCK_EXPORT = "Nhập lượng hàng xuất";

        public const string EVENT_CREATE_REPORT = "Tạo báo cáo mới";
        public const string EVENT_CONFIRM_ALARM = "Xác nhận cảnh báo";
        public const string EVENT_SOLVE_ALARM = "Xử lý cảnh báo";
        public const string EVENT_CONFIG_INFOR = "Cấu hình thông tin chung";
        public const string EVENT_CONFIG_PRODUCT = "Cấu hình mặt hàng";
        public const string EVENT_CONFIG_TANK_CREATE = "Tạo bể";
        public const string EVENT_CONFIG_TANK_UPDATE = "Cấu hình bể";
        public const string EVENT_CONFIG_TANK_DELETE = "Xóa bể";
        public const string EVENT_CONFIG_TANKGROUP_CREATE = "Tạo nhóm bể";
        public const string EVENT_CONFIG_TANKGROUP_UPDATE = "Cấu hình nhóm bể";
        public const string EVENT_CONFIG_TANKGROUP_DELETE = "Xóa nhóm bể";
        public const string EVENT_CONFIG_BAREM_IMPORT = "Tạo barem từ Excel";
        public const string EVENT_CONFIG_BAREM_CREATE = "Thêm barem";
        public const string EVENT_CONFIG_BAREM_UPDATE = "Sửa barem";
        public const string EVENT_CONFIG_BAREM_DELETE = "Xóa barem";

        public const string EVENT_CONFIG_CONFIGARM_CREATE = "Tạo họng";
        public const string EVENT_CONFIG_CONFIGARM_UPDATE = "Sửa họng";
        public const string EVENT_CONFIG_CONFIGARM_DELETE = "Xóa họng";
        public const string EVENT_CONFIG_CONFIGARM_IMPORT = "Import họng";

        public const string EVENT_CONFIG_INVOICE_CREATE = "Tạo hóa đơn";
        public const string EVENT_CONFIG_INVOICE_UPDATE = "Sửa hóa đơn";
        public const string EVENT_CONFIG_INVOICE_DELETE = "Xóa hóa đơn";

        public const string EVENT_CONFIG_PRICE_CREATE = "Tạo giá";
        public const string EVENT_CONFIG_PRICE_UPDATE = "Sửa giá";
        public const string EVENT_CONFIG_PRICE_DELETE = "Xóa giá";
        public const string EVENT_CONFIG_PRICE_IMPORT = "Import giá";

        public const string EVENT_CONFIG_PRODUCT_CREATE = "Tạo mặt hàng";
        public const string EVENT_CONFIG_PRODUCT_UPDATE = "Sửa mặt hàng";
        public const string EVENT_CONFIG_PRODUCT_DELETE = "Xóa mặt hàng";
        public const string EVENT_CONFIG_PRODUCT_IMPORT = "Import mặt hàng";

        public const string EVENT_CONFIG_DRIVER_CREATE = "Tạo lái xe";
        public const string EVENT_CONFIG_DRIVER_UPDATE = "Sửa lái xe";
        public const string EVENT_CONFIG_DRIVER_DELETE = "Xóa lái xe";
        public const string EVENT_CONFIG_DRIVER_IMPORT = "Import lái xe";

        public const string EVENT_CONFIG_DENSITY_CREATE = "Tạo tỷ trọng họng";
        public const string EVENT_CONFIG_DENSITY_UPDATE = "Sửa tỷ trọng họng";
        public const string EVENT_CONFIG_DENSITY_DELETE = "Xóa tỷ trọng họng";
        public const string EVENT_CONFIG_DENSITY_IMPORT = "Import tỷ trọng họng";

        public const string EVENT_CONFIG_CUSTOMER_CREATE = "Tạo khách hàng";
        public const string EVENT_CONFIG_CUSTOMER_UPDATE = "Sửa khách hàng";
        public const string EVENT_CONFIG_CUSTOMER_DELETE = "Xóa khách hàng";
        public const string EVENT_CONFIG_CUSTOMER_IMPORT = "Import khách hàng";


        public const string EVENT_CONFIG_DENSITY_CONFIGARM_CREATE = "Cập nhật tỉ trọng họng";
        public const string EVENT_CONFIG_CONFIGARMGROUP_CREATE = "Tạo nhóm họng";
        public const string EVENT_CONFIG_CONFIGARMGROUP_UPDATE = "Cấu hình nhóm họng";
        public const string EVENT_CONFIG_CONFIGARMGROUP_DELETE = "Xóa nhóm họng";

        public const string EVENT_CONFIG_COMMAND_CREATE = "Tạo lệnh";
        public const string EVENT_CONFIG_COMMAND_UPDATE = "Sửa lệnh";
        public const string EVENT_CONFIG_COMMAND_DELETE = "Xóa lệnh";
		public const string EVENT_CONFIG_COMMAND_CANCEL = "Hủy lệnh";
		public const string EVENT_CONFIG_COMMAND_APPROVE = "Duyệt lệnh";
        public const string EVENT_CONFIG_COMMAND_UNAPPROVE = "Huỷ duyệt lệnh";
        public const string EVENT_CONFIG_COMMAND_MOVE = "Chuyển ngày lệnh";
		public const string EVENT_CONFIG_COMMAND_CANCEL_COMPARTMENT = "Duyệt lệnh";

        public const string EVENT_CONFIG_COMMANDDETAIL_CREATE = "Tạo chi tiết lệnh";
        public const string EVENT_CONFIG_COMMANDDETAIL_DELETE = "Xóa chi tiết lệnh";
        public const string EVENT_CONFIG_COMMANDDETAIL_UPDATE = "Cập nhật chi tiết lệnh";

        public const string EVENT_CONFIG_SEAL_CREATE = "Tạo niêm chì";
        public const string EVENT_CONFIG_SEAL_UPDATE = "Sửa niêm chì";
        public const string EVENT_CONFIG_SEAL_DELETE = "Xóa niêm chì";

        public const string EVENT_CONFIG_WAREHOUSE_CREATE = "Tạo kho";
        public const string EVENT_CONFIG_WAREHOUSE_UPDATE = "Sửa kho";
        public const string EVENT_CONFIG_WAREHOUSE_DELETE = "Xóa kho";

        public const string EVENT_CONFIG_VEHICEL_CREATE = "Tạo phương tiện";
        public const string EVENT_CONFIG_VEHICEL_UPDATE = "Sửa phương tiện";
        public const string EVENT_CONFIG_VEHICEL_DELETE = "Xóa phương tiện";
        public const string EVENT_CONFIG_VEHICEL_IMPORT = "Import phương tiện";

        public const string EVENT_CONFIG_CARD_CREATE = "Tạo thẻ";
        public const string EVENT_CONFIG_CARD_UPDATE = "Sửa thẻ";
        public const string EVENT_CONFIG_CARD_DELETE = "Xóa thẻ";
        public const string EVENT_CONFIG_CARD_IMPORT = "Import thẻ";

        public const string EVENT_CONFIG_SHIP_CREATE = "Tạo phương tiện thủy";
        public const string EVENT_CONFIG_SHIP_UPDATE = "Sửa phương tiện thủy";
        public const string EVENT_CONFIG_SHIP_DELETE = "Xóa phương tiện thủy";
        public const string EVENT_CONFIG_SHIP_IMPORT = "Import phương tiện thủy";

        public const string EVENT_CONFIG_LOCATION_CREATE = "Tạo địa điểm";
        public const string EVENT_CONFIG_LOCATION_UPDATE = "Sửa địa điểm";
        public const string EVENT_CONFIG_LOCATION_DELETE = "Xóa địa điểm";

        // cái này có thể không dùng
        public const string EVENT_CONFIG_CLOCK_CREATE = "Tạo phương tiện";
        public const string EVENT_CONFIG_CLOCK_UPDATE = "Sửa phương tiện";
        public const string EVENT_CONFIG_CLOCK_DELETE = "Xóa phương tiện";

        public const string EVENT_CONFIG_ALARM = "Cấu hình cảnh báo";
        public const string EVENT_CONFIG_ALARM_SOUND = "Sửa âm thanh cảnh báo";
        public const string EVENT_CONFIG_CREATE_TANK_MANUAL = "Đặt thông số đo tay";
        public const string EVENT_CONFIG_UPDATE_TANK_MANUAL = "Sửa thông số đo tay";
        public const string EVENT_CONFIG_DELETE_TANK_MANUAL = "Xóa thông số đo tay";
        public const string EVENT_CONFIG_HOME_IMAGE = "Đổi ảnh chính";
        public const string EVENT_CONFIG_LOGO = "Đổi logo";
        public const string EVENT_CONFIG_COMP_NAME_LOGO = "Đổi tên công ty và đơn vị";
		public const string EVENT_CONFIG_EXPORT_MODE = "Đổi cách thức xuất hàng";

		// Information column Import
		public const int Import_Column_Customer = 12;
        public const int Import_Column_Driver = 8;
        public const int Import_Column_ConfigArm = 8;
        public const int Import_Column_Card = 4;
        public const int Import_Column_Vehicle = 17;
        public const int Import_Column_Barem = 3;
        public const int Import_Column_Price = 7;
       



        public const string EVENT_MANAGER_USER = "Quản lý User";
        public const string EVENT_MANAGER_USER_CREATE = "Tạo mới User";
        public const string EVENT_MANAGER_USER_UPDATE = "Sửa User";
        public const string EVENT_MANAGER_USER_DELETE = "Xóa User";
        public const string EVENT_MANAGER_USERGROUP_CREATE = "Tạo mới nhóm User";
        public const string EVENT_MANAGER_USERGROUP_UPDATE = "Sửa nhóm User";
        public const string EVENT_MANAGER_USERGROUP_DELETE = "Xóa nhóm User";
        public const string EVENT_MANAGER_TANK_CREATE = "Tạo mới bể";
        public const string EVENT_MANAGER_TANK_UPDATE = "Sửa thông tin bể";
        public const string EVENT_MANAGER_TANK_DELETE = "Xóa bể";
        public const string EVENT_MANAGER_TANKGROUP_CREATE = "Tạo mới nhóm bể";
        public const string EVENT_MANAGER_TANKGROUP_UPDATE = "Sửa thông tin nhóm bể";
        public const string EVENT_MANAGER_TANKGROUP_DELETE = "Xóa nhóm bể";
        public const string EVENT_CONFIG_SELECTED_FIELD = "Sửa trường hiển thị";

        public const string CONFIG_HEADER_REPOSITORY_NAME = "HEADER_REPOSITORY_NAME";
        public const string CONFIG_HEADER_CUSTOMER_NAME = "HEADER_REPOSITORY_NAME";
        public const string CONFIG_HOME_IMAGE = "HOME_IMAGE";
        public const string CONFIG_LOGO = "LOGO";
        public const string CONFIG_SOUND_ALARM_FILE = "SOUND_ALARM_FILE";
        public const string CONFIG_SELECTED_FIELD = "SELECTED_FIELD";
        public const string CONFIG_COMP_NAME = "COMP_NAME";
        public const string CONFIG_UNIT = "UNIT";
        public const string CONFIG_SELECTED_LIVEDATAARM_FIELD = "SELECTED_LIVEDATAARM_FIELD";
		public const string CONFIG_EXPORT_MODE = "EXPORT_MODE";


		public const string MESSAGE_ALERT_UPDATE_SUCCESS = "Cập nhật thành công";
        public const string MESSAGE_ALERT_UPDATE_FAILED = "Cập nhật thất bại";
        public const string MESSAGE_ALERT_INSERT_SUCCESS = "Thêm mới thành công";
        public const string MESSAGE_ALERT_INSERT_FAILED = "Thêm mới thất bại";
        public const string MESSAGE_ALERT_DELETE_SUCCESS = "Xóa thành công";
        public const string MESSAGE_ALERT_DELETE_FAILED = "Xóa thất bại";

        public const string MESSAGE_ALERT_ERR_APPROVE = "Phương tiện đang có lệnh (chờ/đang) xuất. Vui lòng kiểm tra lại";
        public const string MESSAGE_ALERT_OK_APPROVE = "Duyệt thành công";

        public const string MESSAGE_ALERT_OK_UNAPPROVE = "Huỷ Duyệt thành công";

        public const string MESSAGE_ALERT_SEAL_SUCCESS = "Niêm chì thành công";
        public const string MESSAGE_ALERT_SEAL_FAILED = "Niêm chì thất bại";

        public const string MESSAGE_ALERT_BAREM_EXISTED = "Đã có chiều cao này";

        public const string DIMENSION_L = "L";
        public const string DIMENSION_MM = "mm";
        public const string DIMENSION_TEMPERATURE = "<sup>o</sup>C";
        public const string DIMENSION_FLOW = "m<sup>3</sup>/h";
        public const string DIMENSION_DENSITY = "Kg/l";
        public const string DIMENSION_LEVEL_RATE = "m/h";
        public const string DIMENSION_MASS_RATE = "Kg/h";
        public const string DIMENSION_MASS = "Kg";

        public const byte Command_Flag_Register = 99;//Đăng ký lệnh 
		public const byte Command_Flag_Approved = 0;//Đã duyệt lệnh 
		public const byte Command_Flag_PrepareExport = 1; //Chuẩn bị xuất
        public const byte Command_Flag_Exporting = 2; //Đang xuất
        public const byte Command_Flag_Exported = 3;//Xuất xong
        public const byte Command_Flag_StopPressing = 4; //Dừng ép
        public const byte Command_Flag_InputCancel = 5; //Hủy lệnh
        public const byte Command_Flag_InputHand = 6; //Nhập tay
        public const byte Command_Flag_Seal = 7; //Niêm chì
        public const byte Command_Flag_Invoice = 8; //Xuất hóa đơn
        public const byte Command_Flag_ChangeDate = 9; //Chuyển ngày
        public const byte Command_Flag_CardError = 10; //Hủy thẻ
        public const byte Command_Flag_Complete = 68; //Hoàn thành

        //Name session is here 

        public const string Session_WareHouse = "WareHouseCode";
        public const byte Session_WareHouseCode = 0;
        public const string Session_WareHouseName = "WareHouseName";
        public const string Session_TitleReportCompanyName = "TitleReportCompanyName"; //Thông tin về đơn vị trên tiêu đề báo cáo
        public const string Session_TitleProviceName = "TitleProviceName"; //Thông tin về đơn vị trên tiêu đề báo cáo
        public const string Session_TitleCompanyAddress = "CompanyAddress"; //Địa chỉ công ty
        public const string Session_TitleCompanyPhone = "CompanyPhone"; //Điện thoại công ty
        public const string Session_TitleCompanyFax = "CompanyFax"; //Fax công ty
        public const string Session_TitleIdentificationNumber = "IdentificationNumber"; // chứng minh thư của lái xe

        public const string Session_ConfigArmGroup = "ConfigArmGroup"; // ID Nhóm họng xuất
        public const string Session_ConfigArmGroupName = "ConfigArmGroupName";


        //Tạm thời xét nội dung ở đây

        public const string Session_Content_TitleReportCompanyName = "Công ty Than Hòn Gai - TKV"; //Thông tin về đơn vị trên tiêu đề báo cáo
        public const string Session_Content_TitleProviceName = "UBND THÀNH PHỐ QUẢNG NINH"; //Thông tin về đơn vị trên tiêu đề báo cáo
        public const string Session_Content_TitleCompanyAddress = "Km7, Phường Quang Hạnh, Cẩm Phả, tỉnh Quảng Ninh"; //Địa chỉ công ty
        public const string Session_Content_TitleCompanyPhone = "033.3862665"; //Điện thoại công ty
        public const string Session_Content_TitleCompanyFax = "033.3737923"; //Fax công ty

        // set giá trị lớn nhất cho số chứng từ CertificateNumber Command 
        public const string Session_CertificateNumber = "CertificateNumber"; 
    }
}
