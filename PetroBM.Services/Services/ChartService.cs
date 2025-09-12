using PetroBM.Common.Util;
using PetroBM.Data;
using PetroBM.Data.Repositories;
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

    public interface IChartService
    {
        DataTable Chart_TankAndWareHouse(DateTime? fromdate, DateTime? todate,byte wareHouseCode, int tankgroupid);
        DataTable Chart_ExportError(DateTime? fromdate, DateTime? todate, byte wareHouseCode, byte ArmNo, double? Deviation);
        DataTable Chart_DiffExportDay(DateTime? fromdate, DateTime? todate, byte wareHouseCode, string ProductCode, double? Deviation);
        DataTable Chart_UsagePerformance(DateTime? fromdate, DateTime? todate, byte wareHouseCode,int tankGroupId);
        DataTable Char_HistoricalExportDataLogArm(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, decimal? workOrder);
        DataTable Chart_DiffExportDay_Calculate_Pie_Chart(DateTime? fromdate, DateTime? todate, byte wareHouseCode, string ProductCode, double? Deviation);
        DataTable Chart_ExportError_PieChart(DateTime? fromdate, DateTime? todate, byte wareHouseCode, byte ArmNo, double? Deviation);
    }

    public class ChartService : IChartService
    {
        public DataTable Chart_DiffExportDay(DateTime? fromdate, DateTime? todate, byte wareHouseCode, string ProductCode, double? Deviation)
        {
            if (Deviation == null)
                Deviation = 100000; // Mặc định lấy tất cả
            var ds = new DataSet();
            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Chart_DiffExportDay_By_WareHouseCode_ProductCode_FromDate_ToDate";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("FromDate", fromdate);
                        SqlParameter param2 = new SqlParameter("ToDate", todate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouseCode);
                        SqlParameter param4 = new SqlParameter("ProductCode", ProductCode?? "");
                        SqlParameter param5 = new SqlParameter("Deviation", Deviation);
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

        public DataTable Chart_ExportError(DateTime? fromdate, DateTime? todate, byte wareHouseCode, byte ArmNo, double? Deviation)
        {

            if (Deviation == null)
                Deviation = 100000; // Mặc định lấy tất cả
            var ds = new DataSet();
            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Chart_ExportError_By_WareHouse_ConfigArm";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("FromDate", fromdate);
                        SqlParameter param2 = new SqlParameter("ToDate", todate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouseCode);
                        SqlParameter param4 = new SqlParameter("ArmNo", ArmNo);
                        SqlParameter param5 = new SqlParameter("Deviation", Deviation);
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

        public DataTable Chart_TankAndWareHouse(DateTime? fromdate, DateTime? todate, byte wareHouseCode, int tankgroupid)
        {

            var ds = new DataSet();
            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Chart_TankAndWareHouse_By_TankGroup_WareHouse";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("FromDate", fromdate);
                        SqlParameter param2 = new SqlParameter("ToDate", todate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouseCode);
                        SqlParameter param4 = new SqlParameter("TankGroupId", tankgroupid);
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);
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

        public DataTable Chart_UsagePerformance(DateTime? fromdate, DateTime? todate, byte wareHouseCode,int tankGroupId)
        {
            var ds = new DataSet();
            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Chart_UsagePerformance_By_WareHouse";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("FromDate", fromdate);
                        SqlParameter param2 = new SqlParameter("ToDate", todate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouseCode);
                        SqlParameter param4 = new SqlParameter("TankGrpId", tankGroupId);
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);
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

        public DataTable Char_HistoricalExportDataLogArm(DateTime? startDate, DateTime? endDate, byte? wareHouse, byte? armNo, string productCode, decimal? workOrder)
        {
            var ds = new DataSet();
            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Chart_HistoricalExportDataLogArm";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("StartDate", startDate);
                        SqlParameter param2 = new SqlParameter("EndDate", endDate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouse);
                        SqlParameter param4 = new SqlParameter("ArmNo", armNo);
                        SqlParameter param5 = new SqlParameter("ProductCode", productCode);
                        SqlParameter param6 = new SqlParameter("WorkOrder", workOrder);
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

        public DataTable Chart_DiffExportDay_Calculate_Pie_Chart(DateTime? fromdate, DateTime? todate, byte wareHouseCode, string ProductCode, double? Deviation)
        {
            if (Deviation == null)
                Deviation = 100000; // Mặc định lấy tất cả
            var ds = new DataSet();
            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Chart_DiffExportDay_Calculate_Pie_Chart";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("FromDate", fromdate);
                        SqlParameter param2 = new SqlParameter("ToDate", todate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouseCode);
                        SqlParameter param4 = new SqlParameter("ProductCode", ProductCode ?? "");
                        SqlParameter param5 = new SqlParameter("Deviation", Deviation);
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

        public DataTable Chart_ExportError_PieChart(DateTime? fromdate, DateTime? todate, byte wareHouseCode, byte ArmNo, double? Deviation)
        {

            if (Deviation == null)
                Deviation = 100000; // Mặc định lấy tất cả
            var ds = new DataSet();
            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Chart_ExportError_By_WareHouse_ConfigArm_PieChart";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("FromDate", fromdate);
                        SqlParameter param2 = new SqlParameter("ToDate", todate);
                        SqlParameter param3 = new SqlParameter("WareHouseCode", wareHouseCode);
                        SqlParameter param4 = new SqlParameter("ArmNo", ArmNo);
                        SqlParameter param5 = new SqlParameter("Deviation", Deviation);
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


    }
}
