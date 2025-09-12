using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetroBM.Domain.Entities;

namespace PetroBM.Web.Models
{
    public class UserGroupModel
    {
        public UserGroupModel()
        {
            UserGrp = new MUserGrp();
            UserGrpList = new HashSet<MUserGrp>();
            UserGrpPermissionList = new List<MUserGrpPermission>();
        }

        public IEnumerable<MUserGrp> UserGrpList { get; set; }
        public MUserGrp UserGrp { get; set; }
        public IList<MUserGrpPermission> UserGrpPermissionList { get; set; }
        public IList<MWareHouse> WareHouseList { get; set; }

    }
}