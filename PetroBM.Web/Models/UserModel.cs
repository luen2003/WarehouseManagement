using PagedList;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class UserModel
    {
        public MUser User { get; set; }
        public IPagedList<MUser> ListUser { get; set; }
        public IEnumerable<MUserGrp> ListUserGrp { get; set; }
        public List<int> ListUserGrpId { get; set; }
        public int? UserGrpId { get; set; }
        public int JobTitles { get; set; }
        public string UserID { get; set; }
        public string SerialNumber { get; set; }
    }
}