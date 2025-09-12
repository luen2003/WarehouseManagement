using PagedList;
using PetroBM.Domain.Entities;
using System.Collections.Generic;
using System.Web;
using System.Data;

namespace PetroBM.Web.Models
{
    public class LoadingArmModel
    {
        public LoadingArmModel()
        {
            LiveDataArm = new MLiveDataArm();
            ListSelectedField = new List<string>();
        }

        public MConfigArm ConfigArm { get; set; }
        public IEnumerable<MConfigArm> ListConfigArm { get; set; }       
        public List<string> ListSelectedField { get; set; }
        public MLiveDataArm LiveDataArm { get; set; }
        public IEnumerable<MLiveDataArm> ListLiveDataArm { get; set; }
        public IEnumerable<MLiveDataArm> ListLiveDataArmConfigArm { get; set; }
        public IEnumerable<MLiveDataArm> ListLiveDataArmByArmNo { get; set; }
        public IEnumerable<MLiveDataArm> ListLiveDataArmByWareHouseCode { get; set; }
        public MCommandDetail CommandDetail { get; set; }
        public IEnumerable<MCommandDetail> ListCommandDetailByArmNo { get; set; }
        public IEnumerable<MVehicle> ListVehicle { get; set; }
        public DataTable LiveDataArmData { get; set; }
        public List<MCommandDetail> ListCommandDetail { get; set; }
        public List<MCommand> ListCommand { get; set; }
        public List<MCustomer> ListCustomer { get; set; }

        public byte? WareHouseCode { get; set; }
        public string VehicleNumber { get; set; }
        public byte ArmNo { get; set; }
    }
}