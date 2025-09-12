using PagedList;
using PetroBM.Domain.Entities;
using System.Collections.Generic;

namespace PetroBM.Web.Models
{
    public class ConfigArmGroupModel
    {
        public ConfigArmGroupModel()
        {
            ConfigArmGrp = new MConfigArmGrp();
            ListSelectedField = new List<string>();
        }

        public MConfigArmGrp ConfigArmGrp { get; set; }
        public IPagedList<MConfigArmGrp> ListConfigArmGrp { get; set; }
        public List<string> ListSelectedField { get; set; }
        public List<MWareHouse> ListWareHouse { get; set; }
        public List<MConfigArm> ListConfigArm { get; set; }
        public List<int> ListConfigArmId { get; set; }
        public List<int> ListStartConfigArmId { get; set; }
        public string WareHouseName { get; set; }
        public List<ConfigArmTempModel> ListConfigArmTemps { get; set; }
        public List<ConfigArmTempModel> SelectConfigArmTemps { get; set; }
        public MConfigArm ConfigArm { get; set; }
        public MLiveDataArm LiveDataArm { get; set; }



    }
}