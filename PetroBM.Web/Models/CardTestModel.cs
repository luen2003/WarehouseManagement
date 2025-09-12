using PagedList;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class CardTestModel
    {
        public string CardData { get; set; }
        public long? CardSerial { get; set; }
        public int CurrentFlag { get; set; } // Trạng thái Card Hiện tại đang ở trạng thái nào

    }
}