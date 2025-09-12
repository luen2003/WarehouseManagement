using PagedList;
using PetroBM.Web.Attribute;
using PetroBM.Common.Util;
using PetroBM.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace PetroBM.Web.Models
{
    public class CardController : Controller
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(CardController));

        private readonly ICardService cardService;
        private readonly CardModel cardModel;
        private readonly IWareHouseService warehouseService;
        private readonly ICommandDetailService commandDetailService;
        private readonly BaseService baseService;


        private readonly IConfigurationService configurationService;

        public CardController(IWareHouseService warehouseService, ICardService cardService, CardModel cardModel, IConfigurationService configurationService, ICommandDetailService commandDetailService, BaseService baseService)
        {
            this.cardService = cardService;
            this.cardModel = cardModel;
            this.warehouseService = warehouseService;
            this.commandDetailService = commandDetailService;
            this.configurationService = configurationService;
            this.baseService = baseService;
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        // GET: Card

        public ActionResult Index(int? page, CardModel cardModel)
        {

            log.Info("Start controller index card ");
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONCARD);
            if (checkPermission)
            {

                try
                {
                    cardModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
                    //cardModel.ListCardBySerial = cardService.GetCardBySerial(cardModel.WareHouseCode, cardModel.ActiveStatus, Int32.Parse(cardModel.CardSerial));
                    if (cardModel.WareHouseCode == 0)
                    {
                        foreach (var item in cardModel.ListWareHouse)
                        {
                            cardModel.WareHouseCode = (byte)item.WareHouseCode;
                            break;
                        }
                    }
                    if (cardModel.ActiveStatus == null)
                    {
                        cardModel.ActiveStatus = 2;
                    }

                    int pageNumber = (page ?? 1);
                    if (cardModel.CardSerial == null || cardModel.CardSerial == "")
                    {
                        cardModel.ListCard = cardService.GetCardBySerial(cardModel.WareHouseCode, cardModel.ActiveStatus, 0).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    }
                    else
                    {
                        var cardSerial = Convert.ToInt32(cardModel.CardSerial);
                        cardModel.ListCard = cardService.GetCardBySerial(cardModel.WareHouseCode, cardModel.ActiveStatus, cardSerial).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    }

                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                log.Info("Finish controller index card ");
                return View(cardModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }

        }

        [ChildActionOnly]
        public ActionResult GetCardNumber()
        {
            var count = cardService.GetAllCard().Count();

            return Content(count.ToString());
        }

        // [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Create()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONCARD);
            if (checkPermission)
            {

                cardModel.ListWareHouse = warehouseService.GetAllWareHouse();
                return View(cardModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult Create(CardModel cardModel)
        {

            log.Info("Start controler create card");
            try
            {
                cardModel.Card.InsertUser = HttpContext.User.Identity.Name;
                cardModel.Card.UpdateUser = HttpContext.User.Identity.Name;
                var rs = cardService.CreateCard(cardModel.Card);

                if (rs)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_SUCCESS;
                }
                else
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_FAILED;
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish controler create card");
            return RedirectToAction("Index");
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Edit(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONCARD);
            if (checkPermission)
            {

                cardModel.Card = cardService.FindCardtById(id);
                cardModel.ListWareHouse = warehouseService.GetAllWareHouse();
                return View(cardModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult Edit(CardModel cardModel)
        {
            log.Info("Start controller card edit");
            try
            {
                var card = cardService.FindCardtById(cardModel.Card.ID);
                if (null != card)
                {
                    card.UpdateUser = HttpContext.User.Identity.Name;
                    card.CardData = cardModel.Card.CardData;
                    card.ActiveStatus = cardModel.Card.ActiveStatus;
                    card.CardSerial = cardModel.Card.CardSerial;
                    card.WareHouseCode = cardModel.Card.WareHouseCode;
                    card.UseStatus = cardModel.UseStatus;

                    if (cardModel.CardError == true)
                    {
                        card.UseStatus = 0;
                        cardModel.lastCommandDetail = commandDetailService.GetAllCommandDetailByCardSerial(card.CardSerial ?? 0).OrderByDescending(x => x.TimeOrder).First();
                        commandDetailService.UpdateListCommandDetail_By_CommandID_Flag(cardModel.lastCommandDetail.CommandID, Constants.Command_Flag_CardError, card.UpdateUser);
                    }

                    var rs = cardService.UpdateCard(card);

                    if (rs)
                    {
                        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                    }

                    else
                    {
                        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish controller card edit");
            return RedirectToAction("Index");
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Delete(int id)
        {
            log.Info("Start controller delete card" + id);
            try
            {

                var rs = cardService.DeleteCard(id);

                if (rs == true)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_SUCCESS;
                }
                else
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_FAILED;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish controller delete card" + id);

            return RedirectToAction("Index");
        }
        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult Import(CardModel cardModel)
        {
            log.Info("Start controller import card");
            try
            {

                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;

                var rs = cardService.ImportCard(cardModel.CardFile, HttpContext.User.Identity.Name);

                if (rs)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish controller import card");
            return RedirectToAction("Index");
        }


    }
}