
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Transactions;
using PetroBM.Data;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;


namespace PetroBM.Services.Services
{
    public interface ITankImportService
    {
        bool CreateTankImport(MTankImport tankimport);
        bool UpdateTankImport(MTankImport tankimport);
        IEnumerable<MTankImport> GetTankImportByImportinfoId(byte wareHouseCode, int importinfId);
        IEnumerable<MTankImport> GetListTankImportByWareHouseCode(byte wareHouseCode);

      

    }
    public class TankImportService : ITankImportService
    {
        private readonly ITankImportRepository tankimportRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public TankImportService(ITankImportRepository tankimportRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.tankimportRepository = tankimportRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateTankImport(MTankImport tankimport)
        {
            var rs = false;
            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Insert_MTankImport_From_ImportInfo";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", tankimport.ImportInfoId);
                            SqlParameter param2 = new SqlParameter("WareHouseCode", tankimport.WareHouseCode);
                            SqlParameter param3 = new SqlParameter("TankId", tankimport.TankId);
                            SqlParameter param4 = new SqlParameter("StartDensity", tankimport.StartDensity);
                            SqlParameter param5 = new SqlParameter("ExportFlg", tankimport.ExportFlg);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.Parameters.Add(param4);
                            cmd.Parameters.Add(param5);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception e) { }

            return rs;
        }

        public bool UpdateTankImport(MTankImport tankimport)
        {
            var rs = false;
            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Update_MTankImport";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", tankimport.ImportInfoId);
                            SqlParameter param2 = new SqlParameter("WareHouseCode", tankimport.WareHouseCode);
                            SqlParameter param3 = new SqlParameter("TankId", tankimport.TankId);
                            SqlParameter param4 = new SqlParameter("StartDate", tankimport.StartDate);
                            SqlParameter param5 = new SqlParameter("StartTemperature", tankimport.StartTemperature?? 0);
                            SqlParameter param6 = new SqlParameter("StartProductLevel", tankimport.StartProductLevel?? 0);
                            SqlParameter param7 = new SqlParameter("StartProductVolume", tankimport.StartProductVolume?? 0);
                            SqlParameter param8 = new SqlParameter("StartVCF", tankimport.StartVCF?? 0);
                            SqlParameter param9 = new SqlParameter("StartProductVolume15", tankimport.StartProductVolume15?? 0);
                            SqlParameter param10 = new SqlParameter("EndTemperature", tankimport.EndTemperature?? 0);
                            SqlParameter param11 = new SqlParameter("EndProductLevel", tankimport.EndProductLevel?? 0);
                            SqlParameter param12 = new SqlParameter("EndProductVolume", tankimport.EndProductVolume?? 0);
                            SqlParameter param13 = new SqlParameter("EndDensity", tankimport.EndDensity?? 0);
                            SqlParameter param14 = new SqlParameter("EndVCF", tankimport.EndVCF?? 0);
                            SqlParameter param15 = new SqlParameter("EndProductVolume15", tankimport.EndProductVolume15?? 0);
                            SqlParameter param16 = new SqlParameter("ExportVolume", tankimport.ExportVolume?? 0);
                            SqlParameter param17 = new SqlParameter("ExportVolume15", tankimport.ExportVolume15?? 0);
                            SqlParameter param18 = new SqlParameter("ExportFlg", tankimport.ExportFlg?? false);
                            SqlParameter param19 = new SqlParameter("StartDensity", tankimport.StartDensity?? 0);
                            SqlParameter param20 = new SqlParameter("EndDate", tankimport.EndDate);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.Parameters.Add(param4);
                            cmd.Parameters.Add(param5);
                            cmd.Parameters.Add(param6);
                            cmd.Parameters.Add(param7);
                            cmd.Parameters.Add(param8);
                            cmd.Parameters.Add(param9);
                            cmd.Parameters.Add(param10);
                            cmd.Parameters.Add(param11);
                            cmd.Parameters.Add(param12);
                            cmd.Parameters.Add(param13);
                            cmd.Parameters.Add(param14);
                            cmd.Parameters.Add(param15);
                            cmd.Parameters.Add(param16);
                            cmd.Parameters.Add(param17);
                            cmd.Parameters.Add(param18);
                            cmd.Parameters.Add(param19);
                            cmd.Parameters.Add(param20);
                           // cmd.Parameters.Add(param21);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception e) { }

            return rs;
        }

        public IEnumerable<MTankImport> GetTankImportByImportinfoId(byte wareHouseCode, int importinfId)
        {
            List<MTankImport> lst = new List<MTankImport>();
            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Get_MTankImport_By_ImportInfoId_ListTankId";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter("ImportInfoId", importinfId);
                        SqlParameter param2 = new SqlParameter("WareHouseCode", wareHouseCode);
                        cmd.Parameters.Add(param);
                        cmd.Parameters.Add(param2);
                        SqlDataReader reader = cmd.ExecuteReader();
                      
                        while (reader.Read())
                        {

                            var it = new MTankImport();
                            it.ImportInfoId = int.Parse(reader["ImportInfoId"].ToString());
                            it.WareHouseCode = byte.Parse(reader["WareHouseCode"].ToString());
                            it.TankId = int.Parse(reader["TankId"].ToString());

                            if (reader["StartDate"].ToString() != "")
                            it.StartDate = DateTime.Parse(reader["StartDate"].ToString());

                            if (reader["StartTemperature"].ToString() != "")
                                it.StartTemperature = float.Parse(reader["StartTemperature"].ToString());

                            if (reader["StartProductLevel"].ToString() != "")
                                it.StartProductLevel = float.Parse(reader["StartProductLevel"].ToString());

                            if (reader["StartProductVolume"].ToString() != "")
                                it.StartProductVolume = float.Parse(reader["StartProductVolume"].ToString());

                            if (reader["StartDensity"].ToString() != "")
                                it.StartDensity = float.Parse(reader["StartDensity"].ToString());

                            if (reader["StartVCF"].ToString() != "")
                                it.StartVCF = float.Parse(reader["StartVCF"].ToString());

                            if (reader["StartProductVolume15"].ToString() != "")
                                it.StartProductVolume15 = double.Parse(reader["StartProductVolume15"].ToString());

                            if (reader["EndDate"].ToString() != "")
                                it.EndDate = DateTime.Parse(reader["EndDate"].ToString());

                            if (reader["EndTemperature"].ToString() != "")
                                it.EndTemperature = float.Parse(reader["EndTemperature"].ToString());

                            if (reader["EndProductLevel"].ToString() != "")
                                it.EndProductLevel = float.Parse(reader["EndProductLevel"].ToString());

                            if (reader["EndProductVolume"].ToString() != "")
                                it.EndProductVolume = float.Parse(reader["EndProductVolume"].ToString());

                            if (reader["EndDensity"].ToString() != "")
                            it.EndDensity = float.Parse(reader["EndDensity"].ToString());

                            if (reader["EndVCF"].ToString() != "")
                            it.EndVCF = float.Parse(reader["EndVCF"].ToString());

                            if (reader["EndProductVolume15"].ToString() != "")
                            it.EndProductVolume15 = float.Parse(reader["EndProductVolume15"].ToString());

                            if (reader["ExportVolume"].ToString() != "")
                            it.ExportVolume = float.Parse(reader["ExportVolume"].ToString());


                            if (reader["ExportVolume15"].ToString() != "")
                            it.ExportVolume15 = float.Parse(reader["ExportVolume15"].ToString());

                            if (reader["ExportFlg"].ToString() != "")
                                it.ExportFlg = bool.Parse(reader["ExportFlg"].ToString());
                            lst.Add(it);
                        }

                       
                    }
                    conn.Close();
                }
            }
            return lst;

        }





        public IEnumerable<MTankImport> GetListTankImportByWareHouseCode(byte wareHouseCode)
        {
            List<MTankImport> lst = new List<MTankImport>();
            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "GetList_MTankImport_By_WareHouseCode";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter("WareHouseCode", wareHouseCode);
                        cmd.Parameters.Add(param);
                        SqlDataReader reader = cmd.ExecuteReader();
                      
                        while (reader.Read())
                        {

                            var it = new MTankImport();
                            it.ImportInfoId = int.Parse(reader["ImportInfoId"].ToString());
                            it.WareHouseCode = byte.Parse(reader["WareHouseCode"].ToString());
                            it.TankId = int.Parse(reader["TankId"].ToString());

                            if (reader["StartDate"].ToString() != "")
                            it.StartDate = DateTime.Parse(reader["StartDate"].ToString());

                            if (reader["StartTemperature"].ToString() != "")
                                it.StartTemperature = float.Parse(reader["StartTemperature"].ToString());

                            if (reader["StartProductLevel"].ToString() != "")
                                it.StartProductLevel = float.Parse(reader["StartProductLevel"].ToString());

                            if (reader["StartProductVolume"].ToString() != "")
                                it.StartProductVolume = float.Parse(reader["StartProductVolume"].ToString());

                            if (reader["StartDensity"].ToString() != "")
                                it.StartDensity = float.Parse(reader["StartDensity"].ToString());

                            if (reader["StartVCF"].ToString() != "")
                                it.StartVCF = float.Parse(reader["StartVCF"].ToString());

                            if (reader["StartProductVolume15"].ToString() != "")
                                it.StartProductVolume15 = double.Parse(reader["StartProductVolume15"].ToString());

                            if (reader["EndDate"].ToString() != "")
                                it.EndDate = DateTime.Parse(reader["EndDate"].ToString());

                            if (reader["EndTemperature"].ToString() != "")
                                it.EndTemperature = float.Parse(reader["EndTemperature"].ToString());

                            if (reader["EndProductLevel"].ToString() != "")
                                it.EndProductLevel = float.Parse(reader["EndProductLevel"].ToString());

                            if (reader["EndProductLevel"].ToString() != "")
                                it.EndProductVolume = float.Parse(reader["EndProductVolume"].ToString());

                            if (reader["EndDensity"].ToString() != "")
                            it.EndDensity = float.Parse(reader["EndDensity"].ToString());

                            if (reader["EndVCF"].ToString() != "")
                            it.EndVCF = float.Parse(reader["EndVCF"].ToString());

                            if (reader["EndProductVolume15"].ToString() != "")
                            it.EndProductVolume15 = float.Parse(reader["EndProductVolume15"].ToString());

                            if (reader["ExportVolume"].ToString() != "")
                            it.ExportVolume = float.Parse(reader["ExportVolume"].ToString());


                            if (reader["ExportVolume15"].ToString() != "")
                            it.ExportVolume15 = float.Parse(reader["ExportVolume15"].ToString());

                            if (reader["ExportFlg"].ToString() != "")
                                it.ExportFlg = bool.Parse(reader["ExportFlg"].ToString());
                            lst.Add(it);
                        }

                       
                    }
                    conn.Close();
                }
            }
            return lst;

        }
    }
}


