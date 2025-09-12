using PagedList;
using PetroBM.Domain.Entities;
using System.Collections.Generic;
using System.Web;

namespace PetroBM.Web.Models
{
    public class ConfigArmModel
    {
        public ConfigArmModel()
        {
            ConfigArm = new MConfigArm();
            ListSelectedField = new List<string>();
            ListWareHouse = new List<MWareHouse>();
        }

        public byte WareHouseCode { get; set; }
        public int? ActiveStatus { get; set; }
        public string ArmName { get; set; }

        public IEnumerable<MWareHouse> ListWareHouse { get; set; }
        public string WareHouseName { get; set; }
        public MConfigArm ConfigArm { get; set; }
        public IPagedList<MConfigArm> ListConfigArm { get; set; }
        public HttpPostedFileBase ImportFile { get; set; }
        public List<string> ListSelectedField { get; set; }
        public IEnumerable<MTank> ListTank { get; set; }

        public IEnumerable<TankTempModel> ListTankTemps { get; set; }
        public IEnumerable<MProduct> ListProduct { get; set; }
        public MTank Tank { get; set; }
        public MLiveDataArm LiveDataArm { get; set; }
        public IPagedList<MLiveDataArm> ListLiveDataArm { get; set; }
        public IEnumerable<MLiveDataArm> ListLiveDataArmByArmNo { get; set; }
        public MCommandDetail CommandDetail { get; set; }
        public IEnumerable<MCommandDetail> ListCommandDetailByArmNo { get; set; }
        public IEnumerable<MConfigArm> ListCheckConfigArm { get; set; }



    }
}