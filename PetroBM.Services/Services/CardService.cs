using log4net;
using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using PetroBM.Data;

namespace PetroBM.Services.Services
{

    public interface ICardService
    {
        IEnumerable<MCard> GetAllCard();
        IEnumerable<MCard> GetAllCardOrderByName();
        IEnumerable<MCard> GetCard(int cardId);
        IEnumerable<MCard> GetCardSerialByCardData(string cardData);
        IEnumerable<MCard> GetCardDataByCardSerial(long cardSerial);
        IEnumerable<MCard> GetCardDataByVehicleNumber(string cardSerial);
        List<MCard> GetCardBySerial(byte wareHouseCode, int? activeStatus, long? cardSerial);
        List<MCard> GetCardByIdVehicle(int? idVehicle);

        List<MCard> GetAllCardNotUse();
        
        bool CreateCard(MCard card);
        bool UpdateCard(MCard card);
        bool DeleteCard(int id);

        MCard FindCardtById(int id);
        bool ImportCard(HttpPostedFileBase file, string user);

    }

    public class CardService : ICardService
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(CardService));

        private readonly ICardRepository cardRepository;
        private readonly IVehicleRepository vehicleRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;


        public CardService(ICardRepository cardRepository, IVehicleRepository vehicleRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.cardRepository = cardRepository;
            this.vehicleRepository = vehicleRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;

        }

        public bool CreateCard(MCard card)
        {
            var rs = false;
            try
            {
                log.Info("Start CreateCard");
                
                using (TransactionScope ts = new TransactionScope())
                {
                    cardRepository.Add(card);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_CARD_CREATE, card.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish CreateCard");

            return rs;
        }

        public bool DeleteCard(int id)
        {
            MCard card = this.FindCardtById(id);
            var rs = false;
            log.Info("Start DeleteCard");
            
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    card.DeleteFlg = Constants.FLAG_ON;
                    cardRepository.Update(card);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_CARD_DELETE, card.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish DeleteCard");
            return rs;
        }

        public IEnumerable<MCard> GetAllCard()
        {
            log.Info("GetAllCard Begin");
            return cardRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(p => p.InsertDate);   
        }

        public IEnumerable<MCard> GetAllCardOrderByName()
        {
            log.Info("GetAllCardOrderByName");
            return cardRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(p => p.ID);
        }

        public bool UpdateCard(MCard card)
        {
            var rs = false;
            log.Info("Start UpdateCard"); 
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    cardRepository.Update(card);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_CARD_UPDATE, card.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish UpdateCard");
            return rs;
        }
        public IEnumerable<MCard> GetCard(int cardId)
        {
            log.Info("GetCard");
            return cardRepository.GetMany(br => br.ID == cardId).OrderBy(br => br.ID);
        }
        public MCard FindCardtById(int id)
        {
            log.Info("FindCardById");
            return cardRepository.GetById(id);
        }
        public bool ImportCard(HttpPostedFileBase file, string user)
        {
            var rs = false;
            log.Info("Start importCard" + Path.GetFileName(file.FileName) + user);
            try
            {
     
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    var path = Path.Combine(HttpContext.Current.Server.MapPath(Constants.FILE_PATH), fileName);
                    file.SaveAs(path);

                    var data = new ExcelUtil().ReadFromExcelfile(path, Constants.Import_Column_Card);
                    using (TransactionScope ts = new TransactionScope())
                    {
                        var cards = new List<MCard>();

                        foreach (System.Data.DataRow row in data.Rows)
                        {

                            var card = new MCard();


                            card.WareHouseCode = row.ItemArray[0].ToString();
                            card.CardSerial = int.Parse(row.ItemArray[1].ToString());
                            card.CardData = row.ItemArray[2].ToString();
                            card.ActiveStatus = int.Parse(row.ItemArray[3].ToString());
                            card.InsertUser = user;
                            card.UpdateUser = user;
                            card.InsertDate = DateTime.Now;
                            card.UpdateDate = DateTime.Now;
                            card.VersionNo = Constants.VERSION_START;
                            card.DeleteFlg = Constants.FLAG_OFF;
                            cards.Add(card);

                        }

                        cardRepository.AddRange(cards);
                        unitOfWork.Commit();
                        ts.Complete();
                        rs = true;
                    }

                    // Log event
                    eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                        Constants.EVENT_CONFIG_CARD_IMPORT, user);


                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish importCard" + Path.GetFileName(file.FileName) + user);

            return rs;
        }

        public IEnumerable<MCard> GetCardSerialByCardData(string cardData)
        {
            log.Info("GetCardSerialByCardData " + cardData);
            return cardRepository.GetAll().Where(p => p.CardData == cardData).OrderBy(p => p.ID);
        }

        public IEnumerable<MCard> GetCardDataByCardSerial(long cardSerial)
        {
            log.Info("GetCardDataByCardSerial " + cardSerial);
            return cardRepository.GetAll().Where(p => p.CardSerial == cardSerial).OrderBy(p => p.ID);
        }

        public IEnumerable<MCard> GetCardDataByVehicleNumber(string VehicleNumber)
        {
            var vehicle = vehicleRepository.GetAll().Where(p => p.VehicleNumber == VehicleNumber && p.DeleteFlg == false).OrderBy(p => p.ID);
            log.Info("GetCardDataByVehicleNumber " + VehicleNumber);
            return cardRepository.GetAll().Join(vehicle , p => p.ID, v => v.CardID, (p, v) => new { p, v }).Where(x => x.v.VehicleNumber == VehicleNumber).OrderBy(x => x.p.ID).Select(x => x.p).ToList();
        }
        
        public List<MCard> GetCardBySerial(byte wareHouseCode, int? activeStatus, long? cardSerial)
        {

            {
                var lst = new List<MCard>();
                try
                {
                    using (var context = new PetroBMContext())
                    {
                        using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                        {
                            conn.Open();
                            using (var cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = "Select_Card_By_WareHouse";
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter param = new SqlParameter("WareHouseCode", wareHouseCode);
                                SqlParameter param2 = new SqlParameter("ActiveStatus", activeStatus ?? 2);
                                SqlParameter param3 = new SqlParameter("CardSerial", cardSerial ?? 0);
                                cmd.Parameters.Add(param);
                                cmd.Parameters.Add(param2);
                                cmd.Parameters.Add(param3);
                                SqlDataReader reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    var it = new MCard();
                                    it.ID = int.Parse(reader["ID"].ToString());
                                    it.WareHouseCode = reader["WareHouseCode"].ToString();
                                    it.CardData = reader["CardData"].ToString();
                                    it.CardSerial = string.IsNullOrEmpty(reader["CardSerial"].ToString())? (long?)null : long.Parse(reader["CardSerial"].ToString());
                                    it.ActiveStatus = int.Parse(reader["ActiveStatus"].ToString());
                                    it.UseStatus = int.Parse(reader["UseStatus"].ToString());
                                    it.UseUser = reader["UseUser"].ToString();
                                    it.UseDate = reader["UseDate"].ToString();
                                    it.DeleteFlg = bool.Parse(reader["DeleteFlg"].ToString());
                                    lst.Add(it);
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


                return lst;
            }
        }

        public List<MCard> GetCardByIdVehicle(int? idVehicle)
        {

            {
                var lst = new List<MCard>();
                try
                {
                    using (var context = new PetroBMContext())
                    {
                        using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                        {
                            conn.Open();
                            using (var cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = "Select_Card_By_IdVehicle";
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter param = new SqlParameter("IDVehicle", idVehicle); 
                                cmd.Parameters.Add(param);
                                SqlDataReader reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    var it = new MCard();
                                    it.ID = int.Parse(reader["ID"].ToString());
                                    it.WareHouseCode = reader["WareHouseCode"].ToString();
                                    it.CardData = reader["CardData"].ToString();
                                    it.CardSerial = string.IsNullOrEmpty(reader["CardSerial"].ToString()) ? (long?)null : long.Parse(reader["CardSerial"].ToString());
                                    it.ActiveStatus = int.Parse(reader["ActiveStatus"].ToString());
                                    it.UseStatus = int.Parse(reader["UseStatus"].ToString()); 
                                    it.DeleteFlg = bool.Parse(reader["DeleteFlg"].ToString());
                                    lst.Add(it);
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


                return lst;
            }
        }

        public List<MCard> GetAllCardNotUse()
        {

            {
                var lst = new List<MCard>();
                try
                {
                    using (var context = new PetroBMContext())
                    {
                        using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                        {
                            conn.Open();
                            using (var cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = "Select_Card_By_NotUse";
                                cmd.CommandType = CommandType.StoredProcedure;  
                                SqlDataReader reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    var it = new MCard();
                                    it.ID = int.Parse(reader["ID"].ToString());
                                    it.WareHouseCode = reader["WareHouseCode"].ToString();
                                    it.CardData = reader["CardData"].ToString();
                                    it.CardSerial = string.IsNullOrEmpty(reader["CardSerial"].ToString()) ? (long?)null : long.Parse(reader["CardSerial"].ToString());
                                    it.ActiveStatus = int.Parse(reader["ActiveStatus"].ToString());
                                    it.UseStatus = int.Parse(reader["UseStatus"].ToString());
                                    it.DeleteFlg = bool.Parse(reader["DeleteFlg"].ToString());
                                    lst.Add(it);
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


                return lst;
            }
        }



    }
}

