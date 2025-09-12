using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class AccountModel
    {
        public AccountModel()
        {
            User = new MUser();
            ListUser = new List<MUser>();
            ListUserGrp = new List<MUserGrp>();
            AlertMessage = "";
        }

        public MUser User { get; set; }
        public IEnumerable<MUser> ListUser { get; set; }
        public IEnumerable<MUserGrp> ListUserGrp { get; set; }
        public int UserGrpId { get; set; }
        public string AlertMessage { get; set; }
    }
}