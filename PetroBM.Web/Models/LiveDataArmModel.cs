using PagedList;
using PetroBM.Domain.Entities;
using System.Collections.Generic;
using System.Web;

namespace PetroBM.Web.Models
{
    public class LiveDataArmModel
    {
        public LiveDataArmModel()
        {
            LiveDataArm = new MLiveDataArm();
            ListSelectedField = new List<string>();
        }

        public byte? ArmNo{ get; set; }
        public MLiveDataArm LiveDataArm { get; set; }
        public List<MConfigArm> ListMConfigArm { get; set; }
        public IEnumerable<MLiveDataArm> ListLiveDataArm { get; set; }
        public List<MLiveDataArm> LiveDataArmList { get; set; }
        public int? ConfigArmGrpId { get; set; }
        public byte? WareHouseCode { get; set; }
        public List<string> ListSelectedField { get; set; }
        public List<MCommandDetail> ListCommandDetail { get; set; }
        public List<MCommand> ListCommand { get; set; }
        public List<MCustomer> ListCustomer { get; set; }
    }
}