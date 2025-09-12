using log4net;
using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Data;
using PetroBM.Data;
using System.Data.SqlClient;

namespace PetroBM.Services.Services
{
    public interface IWareHouseService
    {
        IEnumerable<MWareHouse> GetAllWareHouse();
        IEnumerable<MWareHouse> GetAllWareHouseOrderByName();
       
        bool CreateWareHouse(MWareHouse warehouse);
        bool UpdateWareHouse(MWareHouse warehouse);
        bool DeleteWareHouse(int id);
        IEnumerable<MWareHouse> GetWareHouse_ByUserName(string username);

        IEnumerable<MWareHouse> GetWareHouse_ByUserName_Menu(string username);

        List<byte>GetListWareHouse_ByUserName(string username);

        MWareHouse FindWareHouseById(int id);
    }

    public class WareHouseService : IWareHouseService
    {

        private readonly IWareHouseRepository warehouseRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;
        ILog log = log4net.LogManager.GetLogger(typeof(WareHouseService));

        public WareHouseService(IWareHouseRepository warehouseRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.warehouseRepository = warehouseRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateWareHouse(MWareHouse warehouse)
        {
            var rs = false;
            try
            {
                log.Info("Start CreateWareHouse");
                using (TransactionScope ts = new TransactionScope())
                {
                    warehouseRepository.Add(warehouse);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_WAREHOUSE_CREATE, warehouse.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish CreateWareHouse");
            return rs;
        }

        public bool DeleteWareHouse(int id)
        {
            MWareHouse warehouse = this.FindWareHouseById(id);

            var rs = false;
            log.Info("Start DeleteWareHouse");
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    warehouse.DeleteFlg = Constants.FLAG_ON;
                    warehouseRepository.Update(warehouse);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_WAREHOUSE_DELETE, warehouse.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish DeleteWareHouse");
            return rs;
        }

        public IEnumerable<MWareHouse> GetAllWareHouse()
        {
            IEnumerable<MWareHouse> wareHouses = null;
            log.Info("Start GetAllWareHouse");
            try
            {
                wareHouses = warehouseRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                    .OrderByDescending(p => p.InsertDate);

            }catch(Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finnish GetAllWareHouse");
            return wareHouses;
        }

        public IEnumerable<MWareHouse> GetAllWareHouseOrderByName()
        {
            return warehouseRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(p => p.WareHouseName);
        }

        public bool UpdateWareHouse(MWareHouse warehouse)
        {
            var rs = false;
            log.Info("Start UpdateWareHouse");
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    warehouseRepository.Update(warehouse);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_WAREHOUSE_UPDATE, warehouse.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish UpdateWareHouse");
            return rs;
        }

        public MWareHouse FindWareHouseById(int id)
        {
            return warehouseRepository.GetById(id);
        }

        public IEnumerable<MWareHouse> GetWareHouse_ByUserName(string username)
        {
            IEnumerable<MWareHouse> wareHouses = null;
            var lst = GetListWareHouse_ByUserName(username);
            wareHouses = GetAllWareHouse().Where(x=> lst.Contains((byte)x.WareHouseCode));
            return wareHouses;
        }

        public IEnumerable<MWareHouse> GetWareHouse_ByUserName_Menu(string username)
        {
            IEnumerable<MWareHouse> wareHouses = null;
            var lst = GetListWareHouse_ByUserName(username);
            wareHouses = GetAllWareHouse().Where(x => lst.Contains((byte)x.WareHouseCode) && x.WareHouseCode == 1);
            return wareHouses;
        }

        /// <summary>
        /// Lấy danh sách các kho theo phân quyền
        /// </summary>
        /// <returns></returns>
        public List<byte> GetListWareHouse_ByUserName(string username)
        {
            var lst = new List<byte>();
            log.Info("Start GetListWareHouse_ByUserName");
            try
            {

                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "GetWareHouse_By_UserName";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("UserName", username);
                            cmd.Parameters.Add(param);
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                lst.Add(byte.Parse(reader["WareHouseCode"].ToString()));
                            }
                        }
                        conn.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish GetListWareHouse_ByUserName");
            return lst;

        }
    }
}