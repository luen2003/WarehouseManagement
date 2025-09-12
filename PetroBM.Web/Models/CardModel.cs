using PagedList;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class CardModel
    {
        public CardModel()
        {
            Card = new MCard();
            //ListCard = new List<MCard>();
            ListSelectedField = new List<string>();
            ListWareHouse = new List<MWareHouse>();
        }
        public int? Id { get; set; }
        public int? ActiveStatus { get; set; }
        public int? UseStatus { get; set; }
        public IEnumerable<MWareHouse> ListWareHouse { get; set; }
        public string WareHouseName { get; set; }

        public byte WareHouseCode { get; set; }
        public MCard Card { get; set; }
        public string CardData { get; set; }
        public string CardSerial { get; set; }
        public IPagedList<MCard> ListCard { get; set; }
        public List<string> ListSelectedField { get; set; }
        public HttpPostedFileBase CardFile { get; set; }
        public bool CardError { get; set; }
        public MCommandDetail lastCommandDetail { get; set; }
    }
}