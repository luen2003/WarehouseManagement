using PetroBM.Common.Util;
using PetroBM.Data;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Services.Services
{
	public interface IReportService
	{
		DataTable IOData(DateTime? startDate, DateTime? endDate, int? tankId, int? tankGrpId, int? productId, byte? wareHouse);
		DataTable ReceiptProductData(DateTime? startDate, DateTime? endDate, int? tankId, byte? wareHouse);
		DataTable TankImportData(int? importId);
		DataTable ClockExportData(int? importId);
		DataTable ExportArmImportData(int importInfoId);
		DataTable TankImportInfoData(int? importId);
		DataTable DataLogData(DateTime? startDate, byte? wareHouse);
		DataTable EventData(DateTime? startDate, DateTime? endDate, string user, int? eventTypeId, out DateTime? outStartDate, out DateTime? outEndDate, byte? wareHouse);
		DataTable ErrorData(DateTime? startDate, DateTime? endDate, int? tankId, out DateTime? outStartDate, out DateTime? outEndDate, byte? wareHouse);
		DataTable WarningData(DateTime? startDate, DateTime? endDate, int? tankId, int? alarmTypeId, out DateTime? outStartDate, out DateTime? outEndDate, byte? wareHouse);
		DataTable TankManualData(DateTime? startDate, DateTime? endDate, int? tankId, out DateTime? outStartDate, out DateTime? outEndDate, byte? wareHouse);
		DataTable DataLogArm(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, decimal? workOrder);
		DataTable HistoryDataLogArm(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, decimal? workOrder, int? cardSerial);
		DataTable DataCommandDetail(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productcode, string vehicle, string customername, byte? typeexport);
		DataTable CommandDetailDifference(DateTime? startDate, DateTime? endDate, byte? wareHouse, string armName, int? productId, string vehicle, int? deviation);

		DataTable Report_Deviation_By_CommandDetail(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, string vehicle, int? deviation, string customer, string customerGroup);

		DataTable Report_Deviation_By_GroupDeviation(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, string vehicle, int? deviation);

		DataTable DataTankLog(DateTime? startDate, DateTime? endDate, byte? wareHouse, int? productId);

		DataTable InputExportExist_DataTankLog(DateTime? startDate, DateTime? endDate, byte? wareHouse, int? productId);

        DataTable WarehouseCard_Tank_Inventory(DateTime? startDate,DateTime? endDate, byte? wareHouse, int? productId);

        string GetTankId(byte? wareHousecode, int? tankgroupId);

		TWarehouseCard WarehouseCard(DateTime? startDate, DateTime? endDate, byte? wareHouse, int? productId);
	}

	public class ReportService : IReportService
	{
		private readonly ITankService tankService;
		private readonly ITankGroupService tankGroupService;
		private readonly ITankLiveRepository tankLiveRepository;
		private readonly IEventService eventService;
		private readonly IAlarmService alarmService;
		private readonly IImportService importService;
		private readonly IWareHouseService warehouseService;
		private readonly IProductService productService;

		public ReportService(IProductService productService, IWareHouseService warehouseService, ITankService tankService, ITankGroupService tankGroupService,
			ITankLiveRepository tankLiveRepository, IEventService eventService, IAlarmService alarmService, IImportService importService)
		{
			this.tankLiveRepository = tankLiveRepository;
			this.tankService = tankService;
			this.tankGroupService = tankGroupService;
			this.eventService = eventService;
			this.alarmService = alarmService;
			this.importService = importService;
			this.warehouseService = warehouseService;
			this.productService = productService;
		}

		public DataTable ReceiptProductData(DateTime? startDate, DateTime? endDate, int? tankId, byte? wareHouse)
		{
			var ds = new DataSet();
			using (var context = new PetroBMContext())
			{
				using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
				{
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = "Report_ProductIn";
						cmd.CommandType = CommandType.StoredProcedure;
						SqlParameter param = new SqlParameter("StartDate", startDate);
						SqlParameter param2 = new SqlParameter("EndDate", endDate);
						SqlParameter param3 = new SqlParameter("TankId", tankId.GetValueOrDefault());
						cmd.Parameters.Add(param);
						cmd.Parameters.Add(param2);
						cmd.Parameters.Add(param3);
						using (var adapter = new SqlDataAdapter(cmd))
						{
							adapter.Fill(ds);
						}
					}
				}
			}
			DataTable dt = new DataTable();
			foreach (DataTable tmp in ds.Tables)
			{
				dt.Merge(tmp);
			}

			return dt;
		}

		public DataTable IOData(DateTime? startDate, DateTime? endDate, int? tankId, int? tankGrpId, int? productId, byte? wareHouse)
		{
			var ds = new DataSet();
			string idList = String.Empty;

			if (tankId != null)
			{
				idList = tankId.ToString();
			}
			else if (tankGrpId != null)
			{
				//Không lấy theo cách này nữa
				//var tankGrp = tankGroupService.FindTankById(tankGrpId.Value);
				//foreach (var tank in tankGrp.MTanks)
				//{
				//    idList += tank.TankId + Constants.SPLIT_CHAR;
				//}

				//Thay thế lấy theo cách mới
				//Lấy các bể theo nhóm
				idList = GetTankId(wareHouse, tankGrpId);
			}

			using (var context = new PetroBMContext())
			{
				using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
				{
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = "Report_ImportExport";
						cmd.CommandType = CommandType.StoredProcedure;
						//SqlParameter param1 = new SqlParameter("Id", id);

						SqlParameter param1 = new SqlParameter("ListTankId", idList);
						SqlParameter param2 = new SqlParameter("ProductId", productId ?? 0);
						SqlParameter param3 = new SqlParameter("StartDate", startDate);
						SqlParameter param4 = new SqlParameter("EndDate", endDate);
						SqlParameter param5 = new SqlParameter("WareHouseCode", wareHouse ?? 0);

						cmd.Parameters.Add(param1);
						cmd.Parameters.Add(param2);
						cmd.Parameters.Add(param3);
						cmd.Parameters.Add(param4);
						cmd.Parameters.Add(param5);

						using (var adapter = new SqlDataAdapter(cmd))
						{
							adapter.Fill(ds);
						}
					}
				}
			}
			DataTable dt = new DataTable();
			foreach (DataTable tmp in ds.Tables)
			{
				dt.Merge(tmp);
			}

			return dt;
		}

		public DataTable DataLogData(DateTime? startDate, byte? wareHouse)
		{

			var ds = new DataSet();
			using (var context = new PetroBMContext())
			{
				using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
				{
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = "Report_Inventory";
						cmd.CommandType = CommandType.StoredProcedure;
						SqlParameter param = new SqlParameter("Date", startDate.ToString());
						SqlParameter param2 = new SqlParameter("WareHouseCode", (byte)wareHouse);

						cmd.Parameters.Add(param);
						cmd.Parameters.Add(param2);

						using (var adapter = new SqlDataAdapter(cmd))
						{
							adapter.Fill(ds);
						}
					}
				}
			}
			DataTable dt = new DataTable();
			foreach (DataTable tmp in ds.Tables)
			{
				dt.Merge(tmp);
			}

			return dt;
		}

		public DataTable EventData(DateTime? startDate, DateTime? endDate, string user, int? eventTypeId, out DateTime? outStartDate, out DateTime? outEndDate, byte? wareHouse)
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("EventId");
			dataTable.Columns.Add("EventName");
			dataTable.Columns.Add("InsertDate");
			dataTable.Columns.Add("InsertUser");
			dataTable.Columns.Add("TypeName");

			var events = eventService.GetEventByTime(startDate, endDate, user, eventTypeId).AsEnumerable();

			if (events.Any())
			{
				if (null == startDate)
				{
					startDate = events.Min(a => a.InsertDate);
				}
				if (null == endDate)
				{
					endDate = events.Max(a => a.InsertDate);
				}
			}

			foreach (var item in events)
			{
				dataTable.Rows.Add(item.EventId, item.EventName, item.InsertDate, item.InsertUser, item.MEventType.TypeName);
			}

			outStartDate = startDate;
			outEndDate = endDate;

			return dataTable;
		}

		public DataTable ErrorData(DateTime? startDate, DateTime? endDate, int? tankId,
			out DateTime? outStartDate, out DateTime? outEndDate, byte? wareHouse)
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("AlarmId");
			dataTable.Columns.Add("Issue");
			dataTable.Columns.Add("Value");
			dataTable.Columns.Add("WareHouseName");
			dataTable.Columns.Add("InsertDate");
			dataTable.Columns.Add("TankName");
			dataTable.Columns.Add("AlarmType");

			var errors = alarmService.GetAlarmByTime(startDate, endDate, tankId, Constants.ALARM_TYPE_ERROR, wareHouse).AsEnumerable();

			if (errors.Any())
			{
				if (null == startDate)
				{
					startDate = errors.Min(a => a.InsertDate);
				}
				if (null == endDate)
				{
					endDate = errors.Max(a => a.InsertDate);
				}
			}

			foreach (var item in errors)
			{
				var warehousename = "";
				var objtankname = tankService.GetAllTank().Where(x => x.TankId == item.TankId).ToList();
				var objtypename = alarmService.GetAllAlarmType().Where(x => x.TypeId == item.TypeId).ToList();
				var objwarehouse = warehouseService.GetAllWareHouse().Where(x => x.WareHouseCode == item.WareHouseCode).ToList();
				foreach (var it in objwarehouse)
				{
					warehousename = it.WareHouseName;
				}
				foreach (var items in objtankname)
				{
					var tankname = items.TankName;
					foreach (var itemalarmtype in objtypename)
					{

						var typename = itemalarmtype.TypeName;
						dataTable.Rows.Add(item.AlarmId, item.Issue, item.Value, wareHouse,
		  item.InsertDate.ToString(Constants.DATE_FORMAT), tankname, typename);
					}

				}

			}

			outStartDate = startDate;
			outEndDate = endDate;

			return dataTable;
		}


		public DataTable WarningData(DateTime? startDate, DateTime? endDate, int? tankId, int? alarmTypeId,
			out DateTime? outStartDate, out DateTime? outEndDate, byte? wareHouse)
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("AlarmId");
			dataTable.Columns.Add("Issue");
			dataTable.Columns.Add("Value");
			dataTable.Columns.Add("InsertDate");
			dataTable.Columns.Add("TankName");
			dataTable.Columns.Add("AlarmType");

			var warnings = alarmService.GetAlarmByTime(startDate, endDate, tankId, alarmTypeId, wareHouse)
				.Where(a => a.TypeId != 1).AsEnumerable();

			if (warnings.Any())
			{
				if (null == startDate)
				{
					startDate = warnings.Min(a => a.InsertDate);
				}
				if (null == endDate)
				{
					endDate = warnings.Max(a => a.InsertDate);
				}
			}

			foreach (var item in warnings)
			{
				var objtankname = tankService.GetAllTank().Where(x => x.TankId == item.TankId).ToList();
				var objtypename = alarmService.GetAllAlarmType().Where(x => x.TypeId == item.TypeId).ToList();
				foreach (var items in objtankname)
				{
					var tankname = items.TankName;
					foreach (var itemalarmtype in objtypename)
					{
						var typename = itemalarmtype.TypeName;
						dataTable.Rows.Add(item.AlarmId, item.Issue, item.Value,
		   item.InsertDate, tankname, typename);
					}

				}

			}

			outStartDate = startDate;
			outEndDate = endDate;

			return dataTable;
		}

		public DataTable TankManualData(DateTime? startDate, DateTime? endDate, int? tankId, out DateTime? outStartDate, out DateTime? outEndDate, byte? wareHouse)
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("TankName");
			dataTable.Columns.Add("InsertDate");
			dataTable.Columns.Add("InsertUser");
			dataTable.Columns.Add("WaterLevel");
			dataTable.Columns.Add("TotalLevel");
			dataTable.Columns.Add("AvgTemperature");
			dataTable.Columns.Add("LogWaterLevel");
			dataTable.Columns.Add("LogTotalLevel");
			dataTable.Columns.Add("LogAvgTemperature");
			dataTable.Columns.Add("LogInsertDate");

			var tankManual = tankService.GetTankManualByTime(tankId, startDate, endDate).Where(x => x.WareHouseCode == wareHouse);

			if (tankManual.Any())
			{
				if (null == startDate)
				{
					startDate = tankManual.Min(a => a.InsertDate);
				}
				if (null == endDate)
				{
					endDate = tankManual.Max(a => a.InsertDate);
				}
			}

			foreach (var item in tankManual)
			{
				dataTable.Rows.Add(item.MTank != null ? item.MTank.TankName : "", item.InsertDate, item.InsertUser, item.WaterLevel, item.TotalLevel,
					item.AvgTemperature, item.LogWaterLevel, item.LogTotalLevel, item.LogAvgTemperature, item.LogInsertDate);
			}

			outStartDate = startDate;
			outEndDate = endDate;

			return dataTable;
		}

        public DataTable TankExportData(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productcode, string vehicle, string customername, byte? typeexport)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("OutTime",typeof(DateTime));
            dataTable.Columns.Add("WareHouseName", typeof(string));
            dataTable.Columns.Add("WorkOrder", typeof(string));
            dataTable.Columns.Add("ExportNo", typeof(string));
            dataTable.Columns.Add("ProductCode", typeof(string));
            dataTable.Columns.Add("ProductName", typeof(string));
            dataTable.Columns.Add("VehicleNumber", typeof(string));
            dataTable.Columns.Add("CustomerName", typeof(string));
            dataTable.Columns.Add("Quantity", typeof(long));
            dataTable.Columns.Add("ListVolume", typeof(string));

            dataTable = DataTankExport(startDate, endDate, wareHouse, armNo, productcode, vehicle, customername, typeexport);
            
                return dataTable;
        }

		public DataTable TankImportData(int? importId)
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("StartDate", typeof(DateTime));
			dataTable.Columns.Add("StartTemperature", typeof(Single));
			dataTable.Columns.Add("StartProductLevel", typeof(Single));
			dataTable.Columns.Add("StartProductVolume", typeof(double));
			dataTable.Columns.Add("StartDensity", typeof(Single));
			dataTable.Columns.Add("StartVCF", typeof(Single));
			dataTable.Columns.Add("StartProductVolume15", typeof(double));
			dataTable.Columns.Add("EndDate", typeof(DateTime));
			dataTable.Columns.Add("EndTemperature", typeof(Single));
			dataTable.Columns.Add("EndProductLevel", typeof(Single));
			dataTable.Columns.Add("EndProductVolume", typeof(double));
			dataTable.Columns.Add("EndDensity", typeof(Single));
			dataTable.Columns.Add("EndVCF", typeof(Single));
			dataTable.Columns.Add("EndProductVolume15", typeof(double));
			dataTable.Columns.Add("ExportVolume", typeof(double));
			dataTable.Columns.Add("ExportVolume15", typeof(double));
			dataTable.Columns.Add("ExportFlg", typeof(bool));
			dataTable.Columns.Add("TankName", typeof(string));
			dataTable.Columns.Add("TankId", typeof(int));


			var tankImportList = importService.GetTankImportById(importId.GetValueOrDefault());
			var importInfo = importService.GetImportInfo(importId.GetValueOrDefault());

			foreach (var item in tankImportList)
			{
				if (importInfo.StartFlag == Constants.FLAG_OFF)
				{
					item.StartDate = null;
					item.StartTemperature = null;
					item.StartProductLevel = null;
					item.StartProductVolume = null;
					item.StartDensity = null;
					item.StartVCF = null;
					item.StartProductVolume15 = null;
				}

				if (importInfo.EndFlag == Constants.FLAG_OFF)
				{
					item.EndDate = null;
					item.EndTemperature = null;
					item.EndProductLevel = null;
					item.EndProductVolume = null;
					item.EndDensity = null;
					item.EndVCF = null;
					item.EndProductVolume15 = null;
				}

				dataTable.Rows.Add(item.StartDate, item.StartTemperature, item.StartProductLevel, item.StartProductVolume, item.StartDensity,
					item.StartVCF, item.StartProductVolume15, item.EndDate, item.EndTemperature, item.EndProductLevel,
					item.EndProductVolume, item.EndDensity, item.EndVCF, item.EndProductVolume15, item.ExportVolume,
					item.ExportVolume15, item.ExportFlg, item.MTank != null ? item.MTank.TankName : "", item.MTank != null ? item.MTank.TankId : 0);
			}

			return dataTable;
		}

		public DataTable ClockExportData(int? importId)
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ClockName", typeof(string));
			dataTable.Columns.Add("StartVtt", typeof(Decimal));
			dataTable.Columns.Add("EndVtt", typeof(Decimal));

			var clockExportList = importService.GetAllClockExportById(importId.GetValueOrDefault());

			foreach (var item in clockExportList)
			{
				dataTable.Rows.Add(item.MClock.ClockName, item.StartVtt, item.EndVtt);
			}

			return dataTable;
		}






		public DataTable TankImportInfoData(int? importId)
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Export", typeof(Single));
			dataTable.Columns.Add("Vtt", typeof(Single));
			dataTable.Columns.Add("V15", typeof(Single));
			dataTable.Columns.Add("Temperature", typeof(Single));
			dataTable.Columns.Add("Density", typeof(Single));
			dataTable.Columns.Add("VCF", typeof(Single));
			dataTable.Columns.Add("Vehicle", typeof(string));
			dataTable.Columns.Add("ProductName", typeof(string));
            dataTable.Columns.Add("UpdateDate", typeof(string));

            var tankImportInfo = importService.GetImportInfo(importId.GetValueOrDefault());

			dataTable.Rows.Add(tankImportInfo.Export, tankImportInfo.Vtt, tankImportInfo.V15
				, tankImportInfo.Temperature, tankImportInfo.Density, tankImportInfo.VCF
				, tankImportInfo.Vehicle, tankImportInfo.MProduct != null ? tankImportInfo.MProduct.ProductName : "",tankImportInfo.UpdateDate.ToString("dd/MM/yyyy HH:mm:ss"));

			return dataTable;
		}

		public DataTable DataLogArm(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, decimal? workOrder)
		{
			var ds = new DataSet();

			using (var context = new PetroBMContext())
			{
				using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
				{
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = "Report_HistoryExport";
						cmd.CommandType = CommandType.StoredProcedure;
						//SqlParameter param1 = new SqlParameter("Id", id);
						SqlParameter param1 = new SqlParameter("StartDate", startDate.ToString());
						SqlParameter param2 = new SqlParameter("EndDate", endDate.ToString());

						SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
						SqlParameter param4 = new SqlParameter("ArmNo", armNo ?? 0);
						SqlParameter param5 = new SqlParameter("WorkOrder", workOrder ?? 0);
						cmd.Parameters.Add(param1);
						cmd.Parameters.Add(param2);
						cmd.Parameters.Add(param3);
						cmd.Parameters.Add(param4);
						cmd.Parameters.Add(param5);

						using (var adapter = new SqlDataAdapter(cmd))
						{
							adapter.Fill(ds);
						}
					}
				}
			}
			DataTable dt = new DataTable();
			foreach (DataTable tmp in ds.Tables)
			{
				dt.Merge(tmp);
			}

			return dt;
		}

		public DataTable HistoryDataLogArm(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, decimal? workOrder, int? cardSerial)
		{
			var ds = new DataSet();

			using (var context = new PetroBMContext())
			{
				using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
				{
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = "Report_HistoryExport_By_DataLogArm";
						cmd.CommandType = CommandType.StoredProcedure;
						SqlParameter param1 = new SqlParameter("StartDate", startDate);
						SqlParameter param2 = new SqlParameter("EndDate", endDate);
						SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
						SqlParameter param4 = new SqlParameter("ArmNo", armNo ?? 0);
						SqlParameter param5 = new SqlParameter("WorkOrder", workOrder ?? 0);
						SqlParameter param6 = new SqlParameter("CardSerial", cardSerial);
						cmd.Parameters.Add(param1);
						cmd.Parameters.Add(param2);
						cmd.Parameters.Add(param3);
						cmd.Parameters.Add(param4);
						cmd.Parameters.Add(param5);
						cmd.Parameters.Add(param6);

						using (var adapter = new SqlDataAdapter(cmd))
						{
							adapter.Fill(ds);
						}
					}
				}
			}
			DataTable dt = new DataTable();
			foreach (DataTable tmp in ds.Tables)
			{
				dt.Merge(tmp);
			}

			return dt;
		}


		public DataTable DataCommandDetail(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productcode, string vehicle, string customername, byte? typeexport)
		{
			var ds = new DataSet();
			DataTable dt = new DataTable();

			using (var context = new PetroBMContext())
			{
				using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
				{
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = "Report_TankExport";
						cmd.CommandType = CommandType.StoredProcedure;
						SqlParameter param1 = new SqlParameter("StartDate", startDate);
						SqlParameter param2 = new SqlParameter("EndDate", endDate);
						SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
						SqlParameter param4 = new SqlParameter("ArmNo", armNo ?? 0);
						SqlParameter param5 = new SqlParameter("ProductCode", productcode ?? "");
						SqlParameter param6 = new SqlParameter("Vehicle", vehicle ?? "");
						SqlParameter param7 = new SqlParameter("CustomerName", customername ?? "");
						SqlParameter param8 = new SqlParameter("TypeExport", typeexport ?? 2);
						cmd.Parameters.Add(param1);
						cmd.Parameters.Add(param2);
						cmd.Parameters.Add(param3);
						cmd.Parameters.Add(param4);
						cmd.Parameters.Add(param5);
						cmd.Parameters.Add(param6);
						cmd.Parameters.Add(param7);
						cmd.Parameters.Add(param8);
						using (var adapter = new SqlDataAdapter(cmd))
						{
							adapter.Fill(ds);
						}
					}
				}
			}
			dt = new DataTable();
			foreach (DataTable tmp in ds.Tables)
			{
				dt.Merge(tmp);
			}

			return dt;

		}

        public DataTable DataTankExport(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, string vehicle, string customername, byte? typeexport)
        {
            var ds = new DataSet();
            DataTable dt = new DataTable();

            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Report_TankExport2";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("StartDate", startDate);
                        SqlParameter param2 = new SqlParameter("EndDate", endDate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
                        SqlParameter param4 = new SqlParameter("ArmNo", armNo ?? 0);
                        SqlParameter param5 = new SqlParameter("ProductCode", productCode ?? "");
                        SqlParameter param6 = new SqlParameter("Vehicle", vehicle ?? "");
                        SqlParameter param7 = new SqlParameter("CustomerName", customername ?? "");
                        SqlParameter param8 = new SqlParameter("TypeExport", typeexport ?? 2);
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);
                        cmd.Parameters.Add(param5);
                        cmd.Parameters.Add(param6);
                        cmd.Parameters.Add(param7);
                        cmd.Parameters.Add(param8);
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds);
                        }
                    }
                }
            }
            dt = new DataTable();
            foreach (DataTable tmp in ds.Tables)
            {
                dt.Merge(tmp);
            }

            return dt;

        }

        public DataTable CommandDetailDifference(DateTime? startDate, DateTime? endDate, byte? wareHouse, string armName, int? productId, string vehicle, int? deviation)
		{
			var ds = new DataSet();

			DataTable dt = new DataTable();

			using (var context = new PetroBMContext())
			{
				using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
				{
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = "Report_DifferenceTank";
						cmd.CommandType = CommandType.StoredProcedure;
						SqlParameter param1 = new SqlParameter("StartDate", startDate);
						SqlParameter param2 = new SqlParameter("EndDate", endDate);
						SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
						SqlParameter param4 = new SqlParameter("ArmName", armName ?? "");
						SqlParameter param5 = new SqlParameter("ProductId", productId ?? 0);
						SqlParameter param6 = new SqlParameter("Vehicle", vehicle ?? "");
						SqlParameter param7 = new SqlParameter("Deviation", deviation ?? 0);

						cmd.Parameters.Add(param1);
						cmd.Parameters.Add(param2);
						cmd.Parameters.Add(param3);
						cmd.Parameters.Add(param4);
						cmd.Parameters.Add(param5);
						cmd.Parameters.Add(param6);
						cmd.Parameters.Add(param7);
						using (var adapter = new SqlDataAdapter(cmd))
						{
							adapter.Fill(ds);
						}
					}
				}
			}

			foreach (DataTable tmp in ds.Tables)
			{
				dt.Merge(tmp);
			}
			return dt;
		}

		public DataTable DataTankLog(DateTime? startDate, DateTime? endDate, byte? wareHouse, int? productId)
		{
			DataTable dataTable = new DataTable();

			dataTable.Columns.Add("Product", typeof(string));
			dataTable.Columns.Add("Earlyperiod", typeof(string));
			dataTable.Columns.Add("Endperiod", typeof(Single));
			dataTable.Columns.Add("Importeriod", typeof(Single));
			dataTable.Columns.Add("Exporteiod", typeof(Single));
			dataTable.Columns.Add("ExportTank", typeof(Single));
			dataTable.Columns.Add("ExportTypeExport", typeof(Single));
			dataTable.Columns.Add("EndStock", typeof(Single));
			dataTable.Columns.Add("EmptyStock", typeof(Single));
			dataTable.Columns.Add("Difference", typeof(Single));
			if (productId == null)
			{
				productId = 0;

			}
			var objtanklog = tankService.GetAllDataTankLog((byte)(wareHouse), (int)(productId), startDate, endDate).ToList();
			var product = productService.FindProductById((int)productId).ProductName;

			var i = 0;
			foreach (var item in objtanklog)
			{
				var ds = new DataSet();
				string idList = String.Empty;

				if (item.TankId != null)
				{
					idList = item.TankId.ToString();
				}

				using (var context = new PetroBMContext())
				{
					using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
					{
						using (var cmd = conn.CreateCommand())
						{
							cmd.CommandText = "Report_ImportExport";
							cmd.CommandType = CommandType.StoredProcedure;
							//SqlParameter param1 = new SqlParameter("Id", id);
							SqlParameter param1 = new SqlParameter("ListTankId", idList);
							SqlParameter param2 = new SqlParameter("ProductId", productId ?? 0);

							SqlParameter param3 = new SqlParameter("StartDate", startDate);
							SqlParameter param4 = new SqlParameter("EndDate", endDate);
							cmd.Parameters.Add(param1);
							cmd.Parameters.Add(param2);
							cmd.Parameters.Add(param3);
							cmd.Parameters.Add(param4);
							// cmd.Parameters.Add(param5);

							using (var adapter = new SqlDataAdapter(cmd))
							{
								adapter.Fill(ds);
							}
						}
					}
				}

				foreach (DataTable tmp in ds.Tables)
				{
					dataTable.Merge(tmp);
				}

				DataRow row = dataTable.NewRow();
				row["Product"] = product;
				row["Earlyperiod"] = item.ProductVolume;
				row["Endperiod"] = (int)(Convert.ToInt32(item.TotalVolume) - Convert.ToInt32(item.ProductVolume));
				dataTable.Rows.Add(row);
			}

			return dataTable;
		}
		//

		public DataTable Report_Deviation_By_CommandDetail(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, string vehicle, int? deviation, string customer, string customerGroup)
		{
			DataTable dt = new DataTable();

			using (var context = new PetroBMContext())
			{
				using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
				{
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = "Report_DeviationConfigArm_By_CommandDetail";
						cmd.CommandType = CommandType.StoredProcedure;
						SqlParameter param1 = new SqlParameter("StartDate", startDate);
						SqlParameter param2 = new SqlParameter("EndDate", endDate);
						SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
						SqlParameter param4 = new SqlParameter("ArmNo", armNo ?? 0);
						SqlParameter param5 = new SqlParameter("ProductCode", productCode ?? "");
						SqlParameter param6 = new SqlParameter("Vehicle", vehicle ?? "");
						SqlParameter param7 = new SqlParameter("Deviation", deviation ?? -100000);
                        SqlParameter param8 = new SqlParameter("Customer", customer ?? "");
                        SqlParameter param9 = new SqlParameter("CustomerGroup", customerGroup ?? "");

                        cmd.Parameters.Add(param1);
						cmd.Parameters.Add(param2);
						cmd.Parameters.Add(param3);
						cmd.Parameters.Add(param4);
						cmd.Parameters.Add(param5);
						cmd.Parameters.Add(param6);
						cmd.Parameters.Add(param7);
                        cmd.Parameters.Add(param8);
                        cmd.Parameters.Add(param9);
                        using (var adapter = new SqlDataAdapter(cmd))
						{
							adapter.Fill(dt);
						}
					}
				}
			}

			return dt;
		}

        public DataTable Report_BangKeChiTietXuat(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, string vehicle, int? deviation)
        {
            DataTable dt = new DataTable();

            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Report_BangKeChiTietXuat";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("StartDate", startDate);
                        SqlParameter param2 = new SqlParameter("EndDate", endDate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
                        SqlParameter param4 = new SqlParameter("ArmNo", armNo ?? 0);
                        SqlParameter param5 = new SqlParameter("ProductCode", productCode ?? "");
                        SqlParameter param6 = new SqlParameter("Vehicle", vehicle ?? "");
                        SqlParameter param7 = new SqlParameter("Deviation", deviation ?? -100000);

                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);
                        cmd.Parameters.Add(param5);
                        cmd.Parameters.Add(param6);
                        cmd.Parameters.Add(param7);
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }

            return dt;
        }

        public DataTable Report_BangKeChiTietXuatTheoNgan(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, string vehicle, int? deviation)
        {
            DataTable dt = new DataTable();

            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Report_BangKeChiTietXuatTheoNgan";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("StartDate", startDate);
                        SqlParameter param2 = new SqlParameter("EndDate", endDate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
                        SqlParameter param4 = new SqlParameter("ArmNo", armNo ?? 0);
                        SqlParameter param5 = new SqlParameter("ProductCode", productCode ?? "");
                        SqlParameter param6 = new SqlParameter("Vehicle", vehicle ?? "");
                        SqlParameter param7 = new SqlParameter("Deviation", deviation ?? -100000);

                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);
                        cmd.Parameters.Add(param5);
                        cmd.Parameters.Add(param6);
                        cmd.Parameters.Add(param7);
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }

            return dt;
        }

        public DataTable Report_BangKeTHTheoPhuongTien(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, string vehicle, int? deviation, string customer, string customerGroup)
        {
            DataTable dt = new DataTable();

            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Report_BangKeTHTheoPhuongTien";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("StartDate", startDate);
                        SqlParameter param2 = new SqlParameter("EndDate", endDate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
                        SqlParameter param4 = new SqlParameter("ArmNo", armNo ?? 0);
                        SqlParameter param5 = new SqlParameter("ProductCode", productCode ?? "");
                        SqlParameter param6 = new SqlParameter("Vehicle", vehicle ?? "");
                        SqlParameter param7 = new SqlParameter("Deviation", deviation ?? -100000);
                        SqlParameter param8 = new SqlParameter("Customer", customer ?? "");
                        SqlParameter param9 = new SqlParameter("CustomerGroup", customerGroup ?? "");

                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);
                        cmd.Parameters.Add(param5);
                        cmd.Parameters.Add(param6);
                        cmd.Parameters.Add(param7);
                        cmd.Parameters.Add(param8);
                        cmd.Parameters.Add(param9);
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }

            return dt;
        }

        public DataTable Report_BangKeTHTheoPhuongTien_Total(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, string vehicle, int? deviation, string customer, string customerGroup)
        {
            DataTable dt = new DataTable();

            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Report_BangKeTHTheoPhuongTien_Total";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("StartDate", startDate);
                        SqlParameter param2 = new SqlParameter("EndDate", endDate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
                        SqlParameter param4 = new SqlParameter("ArmNo", armNo ?? 0);
                        SqlParameter param5 = new SqlParameter("ProductCode", productCode ?? "");
                        SqlParameter param6 = new SqlParameter("Vehicle", vehicle ?? ""); 
                        SqlParameter param7 = new SqlParameter("Deviation", deviation ?? -100000);
                        SqlParameter param8 = new SqlParameter("Customer", customer ?? "");
                        SqlParameter param9 = new SqlParameter("CustomerGroup", customerGroup ?? "");

                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);
                        cmd.Parameters.Add(param5);
                        cmd.Parameters.Add(param6);
                        cmd.Parameters.Add(param7);
                        cmd.Parameters.Add(param8);
                        cmd.Parameters.Add(param9);
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }

            return dt;
        }

        public DataTable Report_BangKeTHTheoHong(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, string vehicle, int? deviation, string customer, string customerGroup)
        {
            DataTable dt = new DataTable();

            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Report_BangKeTHTheoHong";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("StartDate", startDate);
                        SqlParameter param2 = new SqlParameter("EndDate", endDate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
                        SqlParameter param4 = new SqlParameter("ArmNo", armNo ?? 0);
                        SqlParameter param5 = new SqlParameter("ProductCode", productCode ?? "");
                        SqlParameter param6 = new SqlParameter("Vehicle", vehicle ?? "");
                        SqlParameter param7 = new SqlParameter("Deviation", deviation ?? -100000);
                        SqlParameter param8 = new SqlParameter("Customer", customer ?? "");
                        SqlParameter param9 = new SqlParameter("CustomerGroup", customerGroup ?? "");

                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);
                        cmd.Parameters.Add(param5);
                        cmd.Parameters.Add(param6);
                        cmd.Parameters.Add(param7);
                        cmd.Parameters.Add(param8);
                        cmd.Parameters.Add(param9);
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }

            return dt;
        }

        public DataTable Report_BangKeTHTheoHong_Total(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, string vehicle, int? deviation, string customer, string customerGroup)
        {
            DataTable dt = new DataTable();

            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Report_BangKeTHTheoHong_Total";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("StartDate", startDate);
                        SqlParameter param2 = new SqlParameter("EndDate", endDate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
                        SqlParameter param4 = new SqlParameter("ArmNo", armNo ?? 0);
                        SqlParameter param5 = new SqlParameter("ProductCode", productCode ?? "");
                        SqlParameter param6 = new SqlParameter("Vehicle", vehicle ?? "");
                        SqlParameter param7 = new SqlParameter("Deviation", deviation ?? -100000);
                        SqlParameter param8 = new SqlParameter("Customer", customer ?? "");
                        SqlParameter param9 = new SqlParameter("CustomerGroup", customerGroup ?? "");

                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);
                        cmd.Parameters.Add(param5);
                        cmd.Parameters.Add(param6);
                        cmd.Parameters.Add(param7);
                        cmd.Parameters.Add(param8);
                        cmd.Parameters.Add(param9);
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }

            return dt;
        }

        public DataTable Report_BangKeTHTheoNhomKhach(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, string vehicle, int? deviation, string customer, string CustomerGroup)
        {
            DataTable dt = new DataTable();

            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Report_BangKeTHTheoNhomKhach";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("StartDate", startDate);
                        SqlParameter param2 = new SqlParameter("EndDate", endDate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
                        SqlParameter param4 = new SqlParameter("ArmNo", armNo ?? 0);
                        SqlParameter param5 = new SqlParameter("ProductCode", productCode ?? "");
                        SqlParameter param6 = new SqlParameter("Vehicle", vehicle ?? ""); 
                        SqlParameter param7 = new SqlParameter("Deviation", deviation ?? -100000);
                        SqlParameter param8 = new SqlParameter("Customer", customer ?? "");
                        SqlParameter param9 = new SqlParameter("CustomerGroup", CustomerGroup ?? "");

                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);
                        cmd.Parameters.Add(param5);
                        cmd.Parameters.Add(param6);
                        cmd.Parameters.Add(param7);
                        cmd.Parameters.Add(param8);
                        cmd.Parameters.Add(param9);
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }

            return dt;
        }

        public DataTable Report_BangKeTHTheoNhomKhach_Total(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, string vehicle, int? deviation, string Customer, string CustomerGroup)
        {
            DataTable dt = new DataTable();

            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Report_BangKeTHTheoNhomKhach_Total";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("StartDate", startDate);
                        SqlParameter param2 = new SqlParameter("EndDate", endDate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
                        SqlParameter param4 = new SqlParameter("ArmNo", armNo ?? 0);
                        SqlParameter param5 = new SqlParameter("ProductCode", productCode ?? "");
                        SqlParameter param6 = new SqlParameter("Vehicle", vehicle ?? "");
                        SqlParameter param7 = new SqlParameter("Deviation", deviation ?? -100000);
                        SqlParameter param8 = new SqlParameter("Customer", Customer ?? "");
                        SqlParameter param9 = new SqlParameter("CustomerGroup", CustomerGroup ?? "");

                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);
                        cmd.Parameters.Add(param5);
                        cmd.Parameters.Add(param6);
                        cmd.Parameters.Add(param7);
                        cmd.Parameters.Add(param8);
                        cmd.Parameters.Add(param9);
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }

            return dt;
        }

        public DataTable Report_BangKeTHTheoNhomKhach2(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, string vehicle, int? deviation, string Customer, string CustomerGroup)
        {
            DataTable dt = new DataTable();

            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Report_BangKeTHTheoNhomKhach2";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("StartDate", startDate);
                        SqlParameter param2 = new SqlParameter("EndDate", endDate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
                        SqlParameter param4 = new SqlParameter("ArmNo", armNo ?? 0);
                        SqlParameter param5 = new SqlParameter("ProductCode", productCode ?? "");
                        SqlParameter param6 = new SqlParameter("Vehicle", vehicle ?? "");
                        SqlParameter param7 = new SqlParameter("Deviation", deviation ?? -100000);
                        SqlParameter param8 = new SqlParameter("Customer", Customer ?? "");
                        SqlParameter param9 = new SqlParameter("CustomerGroup", CustomerGroup ?? "");

                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);
                        cmd.Parameters.Add(param5);
                        cmd.Parameters.Add(param6);
                        cmd.Parameters.Add(param7);
                        cmd.Parameters.Add(param8);
                        cmd.Parameters.Add(param9);
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }

            return dt;
        }

        public DataTable Report_BangKeCTRaVaoKho(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, string vehicle, int? deviation, string Customer, string CustomerGroup)
        {
            DataTable dt = new DataTable();

            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Report_BangKeCTRaVaoKho";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("StartDate", startDate);
                        SqlParameter param2 = new SqlParameter("EndDate", endDate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
                        SqlParameter param4 = new SqlParameter("ArmNo", armNo ?? 0);
                        SqlParameter param5 = new SqlParameter("ProductCode", productCode ?? "");
                        SqlParameter param6 = new SqlParameter("Vehicle", vehicle ?? "");
                        SqlParameter param7 = new SqlParameter("Deviation", deviation ?? -100000);
                        SqlParameter param8 = new SqlParameter("Customer", Customer ?? "");
                        SqlParameter param9 = new SqlParameter("CustomerGroup", CustomerGroup ?? "");

                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);
                        cmd.Parameters.Add(param5);
                        cmd.Parameters.Add(param6);
                        cmd.Parameters.Add(param7);
                        cmd.Parameters.Add(param8);
                        cmd.Parameters.Add(param9);
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }

            return dt;
        }

        public DataTable Report_BangKeChiTietXuat_Total(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, string vehicle, int? deviation)
        {
            DataTable dt = new DataTable();

            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Report_BangKeChiTietXuat_Total";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("StartDate", startDate);
                        SqlParameter param2 = new SqlParameter("EndDate", endDate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
                        SqlParameter param4 = new SqlParameter("ArmNo", armNo ?? 0);
                        SqlParameter param5 = new SqlParameter("ProductCode", productCode ?? "");
                        SqlParameter param6 = new SqlParameter("Vehicle", vehicle ?? "");
                        SqlParameter param7 = new SqlParameter("Deviation", deviation ?? -100000);

                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);
                        cmd.Parameters.Add(param5);
                        cmd.Parameters.Add(param6);
                        cmd.Parameters.Add(param7);
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }

            return dt;
        }


        /// <summary>
        /// Báo cáo Nhập,Xuất, Tồn
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <param name="wareHouse">Mã kho</param>
        /// <param name="productId">Hàng hóa</param>
        /// <returns></returns>
        public DataTable InputExportExist_DataTankLog(DateTime? startDate, DateTime? endDate, byte? wareHouse, int? productId)
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ProductName", typeof(string));
			dataTable.Columns.Add("StartExits", typeof(double));
			dataTable.Columns.Add("EmtyExits", typeof(double));
			dataTable.Columns.Add("ExportOnTank", typeof(double));
			dataTable.Columns.Add("ExportFromStop", typeof(double));
			dataTable.Columns.Add("InputPeroid", typeof(double));
			dataTable.Columns.Add("EndEmtyExits", typeof(double));
			dataTable.Columns.Add("EndExits", typeof(double));
			dataTable.Columns.Add("Deviation", typeof(double));

			using (var context = new PetroBMContext())
			{
				using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
				{
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = "Report_ImportExportExist_By_DataTankLog";
						cmd.CommandType = CommandType.StoredProcedure;
						SqlParameter param1 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
						SqlParameter param2 = new SqlParameter("ProductId", productId ?? 0);
						SqlParameter param3 = new SqlParameter("StartDate", startDate);
						SqlParameter param4 = new SqlParameter("EndDate", endDate);
						cmd.Parameters.Add(param1);
						cmd.Parameters.Add(param2);
						cmd.Parameters.Add(param3);
						cmd.Parameters.Add(param4);
						using (var adapter = new SqlDataAdapter(cmd))
						{
							adapter.Fill(dataTable);
						}
					}
				}
			}

			return dataTable;
		}

		public string GetTankId(byte? wareHousecode, int? tankgroupId)
		{
			string listtankid = string.Empty;

			using (var context = new PetroBMContext())
			{
				using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
				{
					conn.Open();
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = "Select_ListTankId_By_WareHouseCode_TankGroupId";
						cmd.CommandType = CommandType.StoredProcedure;
						SqlParameter param1 = new SqlParameter("WareHouseCode", wareHousecode ?? 0);
						SqlParameter param2 = new SqlParameter("TankGroupId", tankgroupId ?? 0);
						cmd.Parameters.Add(param1);
						cmd.Parameters.Add(param2);
						SqlDataReader reader = cmd.ExecuteReader();

						while (reader.Read())
						{
							listtankid += reader["TankId"].ToString() + Constants.SPLIT_CHAR;
						}
					}
					conn.Close();
				}
			}

			return listtankid;
		}

		public DataTable Report_Deviation_By_GroupDeviation(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, string vehicle, int? deviation)
		{
			DataTable dt = new DataTable();

			using (var context = new PetroBMContext())
			{
				using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
				{
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = "Report_DeviationConfigArm_By_GroupDeviation";
						cmd.CommandType = CommandType.StoredProcedure;
						SqlParameter param1 = new SqlParameter("StartDate", startDate);
						SqlParameter param2 = new SqlParameter("EndDate", endDate);
						SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
						SqlParameter param4 = new SqlParameter("ArmNo", armNo ?? 0);
						SqlParameter param5 = new SqlParameter("ProductCode", productCode ?? "");
						SqlParameter param6 = new SqlParameter("Vehicle", vehicle ?? "");
						SqlParameter param7 = new SqlParameter("Deviation", deviation ?? -100000);

						cmd.Parameters.Add(param1);
						cmd.Parameters.Add(param2);
						cmd.Parameters.Add(param3);
						cmd.Parameters.Add(param4);
						cmd.Parameters.Add(param5);
						cmd.Parameters.Add(param6);
						cmd.Parameters.Add(param7);
						using (var adapter = new SqlDataAdapter(cmd))
						{
							adapter.Fill(dt);
						}
					}
				}
			}

			return dt;
		}

		public DataTable ExportArmImportData(int importInfoId)
		{
			DataTable dt = new DataTable();
			using (var context = new PetroBMContext())
			{
				using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
				{
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = "Report_ExportArmImport_By_ImportinfoId";
						cmd.CommandType = CommandType.StoredProcedure;
						SqlParameter param = new SqlParameter("ImportInfoId", importInfoId);
						cmd.Parameters.Add(param);
						using (var adapter = new SqlDataAdapter(cmd))
						{
							adapter.Fill(dt);
						}
					}
				}
			}

			return dt;
		}

        public DataTable WarehouseCard_Tank_Inventory(DateTime? startDate,DateTime? endDate, byte? wareHouse, int? productId)
        { 
            DataTable dt = new DataTable();
            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Report_WarehouseCard_By_Date_Tank_Inventory";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter("StartDate", startDate);
                        SqlParameter param1 = new SqlParameter("EndDate", endDate);
                        SqlParameter param2 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
                        SqlParameter param3 = new SqlParameter("ProductId", productId ?? 0);


                        cmd.Parameters.Add(param);
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);

                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }

            return dt;
        }

        public TWarehouseCard WarehouseCard(DateTime? startDate, DateTime? endDate, byte? wareHouse, int? productId)
		{
			TWarehouseCard wc = new TWarehouseCard();

			using (var context = new PetroBMContext())
			{
				using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
				{
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = "Report_WarehouseCard_By_Date";
						cmd.CommandType = CommandType.StoredProcedure;
						SqlParameter param1 = new SqlParameter("StartDate", startDate);
						SqlParameter param2 = new SqlParameter("EndDate", endDate);
						SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse ?? 0);
						SqlParameter param4 = new SqlParameter("ProductId", productId ?? 0);
						SqlParameter param5 = new SqlParameter("TotalVtt", SqlDbType.Real);
						param5.Direction = ParameterDirection.Output;
						SqlParameter param6 = new SqlParameter("TotalV15", SqlDbType.Real);
						param6.Direction = ParameterDirection.Output;

						cmd.Parameters.Add(param1);
						cmd.Parameters.Add(param2);
						cmd.Parameters.Add(param3);
						cmd.Parameters.Add(param4);
						cmd.Parameters.Add(param5);
						cmd.Parameters.Add(param6);

						using (var adapter = new SqlDataAdapter(cmd))
						{
							adapter.Fill(wc.DT);

							if (param5.Value != DBNull.Value)
							{
								wc.TotalVtt = Convert.ToDouble(param5.Value);
							}
							if (param6.Value != DBNull.Value)
							{
								wc.TotalV15 = Convert.ToDouble(param6.Value);
							}

							if (wc.DT.Rows.Count > 0)
							{
								int numberOfDay = 1;
								double vtt = 0, v15 = 0;
								DataRow tmpRow = wc.DT.Rows[0];
								DateTime tmpDate = Convert.ToDateTime(wc.DT.Rows[0]["TimeOrder"]);

								foreach (DataRow row in wc.DT.Rows)
								{
									if (tmpDate.Date != Convert.ToDateTime(row["TimeOrder"]).Date)
									{
										numberOfDay++;
										tmpDate = Convert.ToDateTime(row["TimeOrder"]);
										vtt += Convert.ToDouble(tmpRow["TotalVtt"]);
										v15 += Convert.ToDouble(tmpRow["TotalV15"]);
									}
									tmpRow = row;
								}

								vtt += Convert.ToDouble(tmpRow["TotalVtt"]);
								v15 += Convert.ToDouble(tmpRow["TotalV15"]);
								wc.AvgTotalVtt = Math.Round(vtt / numberOfDay, 0);
								wc.AvgTotalV15 = Math.Round(v15 / numberOfDay, 0);

								if (tmpRow["AvgDensity"] != DBNull.Value)
								{
									wc.LastDensity = Convert.ToDouble(tmpRow["AvgDensity"]);
								}
								if (tmpRow["VCF"] != DBNull.Value)
								{
									wc.LastVCF = Convert.ToDouble(tmpRow["VCF"]);
								}
								if (tmpRow["OutV15"] != DBNull.Value)
								{
									wc.LastOutV15 = Convert.ToDouble(tmpRow["OutV15"]);
								}
								if (tmpRow["Deviation"] != DBNull.Value)
								{
									wc.LastDeviation = Convert.ToDouble(tmpRow["Deviation"]);
								}
								wc.LastTotalVtt = Convert.ToDouble(tmpRow["TotalVtt"]);
								wc.LastTotalV15 = Convert.ToDouble(tmpRow["TotalV15"]);

								var dayOfMonth = DateTime.DaysInMonth(startDate.Value.Year, startDate.Value.Month);
								var productStoreWastageRate = productService.FindProductById(productId.Value).StoreWastageRate ?? 0;
								wc.StoreWastageRate = wc.AvgTotalV15 * (productStoreWastageRate / 100) * ((double)numberOfDay / dayOfMonth);
								wc.StoreWastageRateV15 = wc.LastTotalV15 - wc.StoreWastageRate;
								wc.StoreWastageRateVtt = wc.StoreWastageRateV15 / wc.LastDensity;
							}
						}
					}
				}
			}

			return wc;
		}
	}
}
