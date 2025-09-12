using PetroBM.Common.Util;
using PetroBM.Data;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace PetroBM.Services.Services
{
    public interface IConfigArmGrpService
    {
        IEnumerable<MConfigArmGrp> GetAllConfigArmGrp();
        IEnumerable<MConfigArmGrp> GetAllConfigArmGrpOrderByName();
        bool CreateConfigArmGrp(MConfigArmGrp configarmgrp);

        bool CreateConfigArmGroup_By_ListConfigArm(MConfigArmGrp configarmgrp,int[] lstconfigarm);

        bool UpdateConfigArmGrp(MConfigArmGrp configarmgrp);
        bool UpdateConfigArmGroup_By_ListConfigArm(MConfigArmGrp configarmgrp, int[] lstconfigarm); 
        bool DeleteConfigArmGrp(int id);

        int[] Get_ListConfigArmId_By_ConfigArmGroupConfigArm_WareHouseCode(int configarmgrpid, byte warehousecode, string user);

        MConfigArmGrp FindConfigArmGrpById(int id);
    }

    public class ConfigArmGrpService : IConfigArmGrpService
    {
        private readonly IConfigArmGrpRepository configarmgrpRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public ConfigArmGrpService(IConfigArmGrpRepository configarmgrpRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.configarmgrpRepository = configarmgrpRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateConfigArmGrp(MConfigArmGrp configarmgrp)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    configarmgrpRepository.Add(configarmgrp);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_CONFIGARM_CREATE, configarmgrp.InsertUser);
            }
            catch (Exception e) { }

            return rs;
        }


        public bool CreateConfigArmGroup_By_ListConfigArm(MConfigArmGrp configarmgrp, int[] lstconfigarm)
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
                            for (int i = 0; i < lstconfigarm.Count(); i++)
                            {
                                cmd.CommandText = "Insert_ConfigArmGrpConfigArm_By_ConfigArmId_WareHouseCode";
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter param = new SqlParameter("ConfigArmGrpId", configarmgrp.ConfigArmGrpId);
                                SqlParameter param2 = new SqlParameter("ConfigArmId", lstconfigarm[i]);
                                SqlParameter param3 = new SqlParameter("WareHouseCode", configarmgrp.WareHouseCode);
                                SqlParameter param4 = new SqlParameter("User", configarmgrp.InsertUser);
                                cmd.Parameters.Add(param);
                                cmd.Parameters.Add(param2);
                                cmd.Parameters.Add(param3);
                                cmd.Parameters.Add(param4);
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                            rs = true;
                        }
                        conn.Close();
                    }
                }

            }
            catch (Exception ex)
            {

                rs = false;
            }

            return rs;
        }

        public bool DeleteConfigArmGrp(int id)
        {
            MConfigArmGrp configarmgrp = this.FindConfigArmGrpById(id);
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    configarmgrp.DeleteFlg = Constants.FLAG_ON;
                    configarmgrpRepository.Update(configarmgrp);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_CONFIGARM_DELETE, configarmgrp.UpdateUser);
            }
            catch { }

            return rs;
        }

        public IEnumerable<MConfigArmGrp> GetAllConfigArmGrp()
        {
            return configarmgrpRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(p => p.InsertDate);
        }

        public IEnumerable<MConfigArmGrp> GetAllConfigArmGrpOrderByName()
        {
            return configarmgrpRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(p => p.ConfigArmName);
        }

        public bool UpdateConfigArmGrp(MConfigArmGrp configarmgrp)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    configarmgrpRepository.Update(configarmgrp);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_CONFIGARM_UPDATE, configarmgrp.UpdateUser);
            }
            catch (Exception e) { }

            return rs;
        }

        public bool UpdateConfigArmGroup_By_ListConfigArm(MConfigArmGrp configarmgrp, int[] lstconfigarm)
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
                            cmd.CommandText = "Delete_ConfigArmGrpConfigArm_By_ConfigArmGrpId_WareHouseCode";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ConfigArmGrpId", configarmgrp.ConfigArmGrpId);
                            SqlParameter param2 = new SqlParameter("WareHouseCode", configarmgrp.WareHouseCode);
                            SqlParameter param3 = new SqlParameter("User", configarmgrp.UpdateUser);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.ExecuteNonQuery();

                            cmd.Parameters.Clear();


                            for (int i = 0; i < lstconfigarm.Count(); i++)
                            {
                                cmd.CommandText = "Update_ConfigArmGrpConfigArm_By_ConfigArmId_WareHouseCode";
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter param4 = new SqlParameter("ConfigArmGrpId", configarmgrp.ConfigArmGrpId);
                                SqlParameter param5 = new SqlParameter("ConfigArmId", lstconfigarm[i]);
                                SqlParameter param6 = new SqlParameter("WareHouseCode", configarmgrp.WareHouseCode);
                                SqlParameter param7 = new SqlParameter("User", configarmgrp.UpdateUser);
                                cmd.Parameters.Add(param4);
                                cmd.Parameters.Add(param5);
                                cmd.Parameters.Add(param6);
                                cmd.Parameters.Add(param7);
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                            rs = true;
                        }
                        conn.Close();
                    }
                }

            }
            catch (Exception ex)
            {

                rs = false;
            }

            return rs;
        }

        public MConfigArmGrp FindConfigArmGrpById(int id)
        {
            return configarmgrpRepository.GetById(id);
        }

        public int[] Get_ListConfigArmId_By_ConfigArmGroupConfigArm_WareHouseCode(int configarmgrpid, byte warehousecode, string user)
        {
            var lstTankId = new List<int>();
            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select_ListConfigArm_By_WareHouseCode_ConfigArmGrpId";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter("ConfigArmGrpId", configarmgrpid);
                        SqlParameter param2 = new SqlParameter("WareHouseCode", warehousecode);
                        SqlParameter param3 = new SqlParameter("User", user);
                        cmd.Parameters.Add(param);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            lstTankId.Add(int.Parse(reader["ConfigArmId"].ToString()));
                        }
                    }
                    conn.Close();
                }
            }
            return lstTankId.ToArray();
        }
    }
}
