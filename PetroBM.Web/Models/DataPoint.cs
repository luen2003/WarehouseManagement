using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PetroBM.Web.Models
{
    [DataContract]
    public class DataPoint
    {
        public DataPoint(double? x, float? y)
        {
            this.x = x;
            this.y = y;
        }

        [DataMember(Name = "x")]
        public double? x;

        [DataMember(Name = "y")]
        public float? y;
    }
}