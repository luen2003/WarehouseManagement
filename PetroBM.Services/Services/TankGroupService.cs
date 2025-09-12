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
    public interface ITankGroupService
    {
        IEnumerable<MTankGrp> GetAllTankGrp();
        IEnumerable<MTankGrp> GetAllTankGrpOrderByName();

        bool CreateTankGrp(MTankGrp tankGrp);
        bool UpdateTankGrp(MTankGrp tankGrp);
        bool DeleteTankGrp(int id);

        MTankGrp FindTankById(int id);
        bool CreateTankGrpTank(int tankgrpid, byte warehousecode, int[] tankid, string user);

        bool Update_TankGrpTank_By_ListTankId_WareHouseCode(int tankgrpid, byte warehousecode, int[] tankid, string user);

        int[] Get_ListTankId_By_TankgroupTank_WareHouseCode(int tankgrpid, byte warehousecode, string user);
    }

    public class TankGroupService : ITankGroupService
    {
        private readonly ITankGroupRepository tankGrpRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;


        public TankGroupService(ITankGroupRepository tankGrpRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.tankGrpRepository = tankGrpRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateTankGrp(MTankGrp tankGrp)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    tankGrpRepository.Add(tankGrp);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                      Constants.EVENT_MANAGER_TANKGROUP_CREATE, tankGrp.InsertUser);
            }
            catch (Exception e) { }

            return rs;
        }

        public bool DeleteTankGrp(int id)
        {
            MTankGrp tankGrp = this.FindTankById(id);
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    tankGrp.DeleteFlg = Constants.FLAG_ON;
                    tankGrpRepository.Update(tankGrp);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_MANAGER_USER_DELETE, tankGrp.UpdateUser);
            }
            catch (Exception e) { }

            return rs;
        }

        public IEnumerable<MTankGrp> GetAllTankGrp()
        {
            return tankGrpRepository.GetAll().Where(tg => tg.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(tg => tg.InsertDate);
        }

        public IEnumerable<MTankGrp> GetAllTankGrpOrderByName()
        {
            return tankGrpRepository.GetAll().Where(tg => tg.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(tg => tg.TanktGrpName);
        }

        public bool UpdateTankGrp(MTankGrp tankGrp)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    tankGrpRepository.Update(tankGrp);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                      Constants.EVENT_MANAGER_TANKGROUP_UPDATE, tankGrp.UpdateUser);
            }
            catch (Exception e) { }

            return rs;
        }

        public MTankGrp FindTankById(int id)
        {
            var property_info = typeof(MTankGrp).GetProperty("TankGrpId");
            return tankGrpRepository.GetById(id);
        }

        public bool CreateTankGrpTank(int tankgrpid, byte warehousecode, int[] Lsttankid, string user)
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
                            for (int i = 0; i < Lsttankid.Count(); i++)
                            {
                                cmd.CommandText = "Insert_TankGroupTank_By_TankId_WareHouseCode";
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter param = new SqlParameter("TankGrpId", tankgrpid);
                                SqlParameter param2 = new SqlParameter("TankId", Lsttankid[i]);
                                SqlParameter param3 = new SqlParameter("WareHouseCode", warehousecode);
                                SqlParameter param4 = new SqlParameter("User", user);
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


      public  bool Update_TankGrpTank_By_ListTankId_WareHouseCode(int tankgrpid, byte warehousecode, int[] Lsttankid, string user)
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
                            cmd.CommandText = "Delete_TankGroupTank_By_TankGrpId_WareHouseCode";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("TankGrpId", tankgrpid);
                            SqlParameter param2 = new SqlParameter("WareHouseCode", warehousecode);
                            SqlParameter param3 = new SqlParameter("User", user);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.ExecuteNonQuery();

                            cmd.Parameters.Clear();


                            for (int i = 0; i < Lsttankid.Count(); i++)
                            {
                                cmd.CommandText = "Update_TankGroupTank_By_TankId_WareHouseCode";
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter param4 = new SqlParameter("TankGrpId", tankgrpid);
                                SqlParameter param5 = new SqlParameter("TankId", Lsttankid[i]);
                                SqlParameter param6 = new SqlParameter("WareHouseCode", warehousecode);
                                SqlParameter param7 = new SqlParameter("User", user);
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

        public int[] Get_ListTankId_By_TankgroupTank_WareHouseCode(int tankgrpid, byte warehousecode, string user)
        {
            var lstTankId = new List<int>();
            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select_ListTank_By_WareHouseCode_TankGrpId";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter("TankGrpId", tankgrpid);
                        SqlParameter param2 = new SqlParameter("WareHouseCode", warehousecode);
                        SqlParameter param3 = new SqlParameter("User", user);
                        cmd.Parameters.Add(param);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            lstTankId.Add(int.Parse(reader["TankId"].ToString()));
                        }
                    }
                    conn.Close();
                }
            }
            return lstTankId.ToArray();
        }

    }
}