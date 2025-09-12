using PagedList;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class SealModel
    {
        public SealModel()
        {
            Seal = new MSeal();
            ListCommand = new List<MCommand>();
            ListCommanDetail = new List<MCommandDetail>();
            ListSelectedField = new List<string>();
        }

        IEnumerable<CSealModel> ListCSeal { get; set; }

        public MSeal Seal { get; set; }

        public IEnumerable<MCommand> ListEnComman { get; set; }
        public List<MCommand> ListCommand { get; set; }
        public List<MCommandDetail> ListCommanDetail { get; set; }
        public IPagedList<MSeal> ListSeal { get; set; }
        public List<Datum> ListProduct { get; set; }

        public int totalsum { get; set; }
        public List<string> ListSelectedField { get; set; }

		public int ExportMode { get; set; }
	}
}