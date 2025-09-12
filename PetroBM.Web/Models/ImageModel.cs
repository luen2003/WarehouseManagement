using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Web.Models
{
    public class MImageModel
    {
        public int ID { get; set; }

        public string ImageCode { get; set; }

        public string ImageName { get; set; }

        public string ImageURL { get; set; }

        public string ImagePosition { get; set; }

        public string ImageUser { get; set; }

        public int ProcessStatus { get; set; }
    }

}
