using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetroBM.Common.Util;
using System.Data;

namespace PetroBM.Domain.Entities
{
    public class TWarehouseCard
	{
		public TWarehouseCard()
		{
			DT = new DataTable();
		}

		public DataTable DT { get; set; }
		public double? StoreWastageRate { get; set; }
		public double? StoreWastageRateVtt { get; set; }
		public double? StoreWastageRateV15 { get; set; }
		public double? TotalVtt { get; set; }
		public double? TotalV15 { get; set; }
		public double? AvgTotalVtt { get; set; }
		public double? AvgTotalV15 { get; set; }

		public double? LastDensity { get; set; }
		public double? LastVCF { get; set; }
		public double? LastOutV15 { get; set; }
		public double? LastDeviation { get; set; }
		public double? LastTotalVtt { get; set; }
		public double? LastTotalV15 { get; set; }
	}
}
