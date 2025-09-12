
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
using PetroBM.Common.Util;

namespace PetroBM.Services.Services
{
    public interface ITankImportTempService
    {
        bool CreateTankImportTemp(MTankImportTemp tankimporttemp);
        bool UpdateTankImportTemp(MTankImportTemp tankimporttemp);  
        IEnumerable<MTankImportTemp> GetTankImportTempByImportinfoId(byte wareHouseCode, int importinfId);

    }

    public class TankImportTempService : ITankImportTempService
    {
        private readonly ITankImportTempRepository tankimporttempRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public TankImportTempService(ITankImportTempRepository tankimporttempRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.tankimporttempRepository = tankimporttempRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateTankImportTemp(MTankImportTemp tankimporttemp)
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
                            cmd.CommandText = "Insert_MTankImportTemp_From_ImportInfo";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", tankimporttemp.ImportInfoId);
                            SqlParameter param2 = new SqlParameter("WareHouseCode", tankimporttemp.WareHouseCode);
                            SqlParameter param3 = new SqlParameter("TankId", tankimporttemp.TankId);
                            SqlParameter param4 = new SqlParameter("StartDensity", tankimporttemp.StartDensity);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.Parameters.Add(param4);
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

        public bool UpdateTankImportTemp(MTankImportTemp tankimporttemp)
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
                            cmd.CommandText = "Update_TankImportTemp";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", tankimporttemp.ImportInfoId);
                            SqlParameter param2 = new SqlParameter("WareHouseCode", tankimporttemp.WareHouseCode);
                            SqlParameter param3 = new SqlParameter("TankId", tankimporttemp.TankId);
                            SqlParameter param4 = new SqlParameter("StartDate", tankimporttemp.StartDate);
                            SqlParameter param5 = new SqlParameter("StartTemperature", tankimporttemp.StartTemperature);
                            SqlParameter param6 = new SqlParameter("StartProductLevel", tankimporttemp.StartProductLevel);
                            SqlParameter param7 = new SqlParameter("StartDensity", tankimporttemp.StartDensity);
                            SqlParameter param8 = new SqlParameter("EndDate", tankimporttemp.EndDate);
                            SqlParameter param9 = new SqlParameter("EndTemperature", tankimporttemp.EndTemperature);
                            SqlParameter param10 = new SqlParameter("EndProductLevel", tankimporttemp.EndProductLevel);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.Parameters.Add(param4);
                            cmd.Parameters.Add(param5);
                            cmd.Parameters.Add(param6);
                            cmd.Parameters.Add(param7);
                            cmd.Parameters.Add(param7);
                            cmd.Parameters.Add(param8);
                            cmd.Parameters.Add(param9);
                            cmd.Parameters.Add(param10);
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

        public IEnumerable<MTankImportTemp> GetTankImportTempByImportinfoId(byte wareHouseCode, int importinfId)
        {
            List<MTankImportTemp> lst = new List<MTankImportTemp>();
            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Get_MTankImportTemp_By_ImportInfoId";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter("ImportInfoId", importinfId);
                        SqlParameter param2 = new SqlParameter("WareHouseCode", wareHouseCode);
                        cmd.Parameters.Add(param);
                        cmd.Parameters.Add(param2);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {

                            var it = new MTankImportTemp();
                            it.ImportInfoId = int.Parse(reader["ImportInfoId"].ToString());
                            it.WareHouseCode = byte.Parse(reader["WareHouseCode"].ToString());
                            it.TankId = int.Parse(reader["TankId"].ToString());

                            if (reader["StartDate"].ToString() != "")
                                it.StartDate = DateTime.Parse(reader["StartDate"].ToString());

                            if (reader["StartTemperature"].ToString() != "")
                                it.StartTemperature = float.Parse(reader["StartTemperature"].ToString());

                            if (reader["StartProductLevel"].ToString() != "")
                                it.StartProductLevel = float.Parse(reader["StartProductLevel"].ToString());


                            if (reader["StartDensity"].ToString() != "")
                                it.StartDensity = float.Parse(reader["StartDensity"].ToString());

                            if (reader["EndDate"].ToString() != "")
                                it.EndDate = DateTime.Parse(reader["EndDate"].ToString());

                            if (reader["EndTemperature"].ToString() != "")
                                it.StartProductLevel = float.Parse(reader["EndTemperature"].ToString());

                            if (reader["EndProductLevel"].ToString() != "")
                                it.StartProductLevel = float.Parse(reader["EndProductLevel"].ToString());
            
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
