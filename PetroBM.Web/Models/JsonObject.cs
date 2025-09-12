using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{

    public class DataValue
    {
        public DataValue() { }
        public DataValue(string Name, int? Value, string Code, string Unit, string Caption,int? EnvirontmentTax)
        {
            this.Name = Name;
            this.Value = Value;
            this.Caption = Caption;
            this.Code = Code;
            this.Unit = Unit;
            this.EnvironmentTax = EnvirontmentTax;
        }

        public string Name { get; set; }
        public int? Value { get; set; }
        public string Code { get; set; }
        public string Caption { get; set; }
        public string Unit { get; set; }
        public int? EnvironmentTax { get; set; }

    }

    public class Datum
    {
        public Datum () {}
        public Datum(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class ListStatus
    {
        public ListStatus() { }
        public ListStatus(string name,byte flag)
        {
            this.name = name;
            this.flag = flag;
        }
        public string name { get; set; }
        public byte flag { get; set; }
    }

    public class DataList
    {
        public DataList() { }
        public DataList(string label,float y)
        {
            this.label = label;
            this.y = y;
        }
        public float y { get; set; }
        public string label { get; set; }
    }

    public class DataList2
    {
        public DataList2() { }
        public DataList2(string indexLabel, float y)
        {
            this.indexLabel = indexLabel;
            this.y = y;
        }
        public float y { get; set; }
        public string indexLabel { get; set; }
    }

    public class TankGrp
    {
        public TankGrp() { }
        public TankGrp(string tankgrpname, int tankgrpid, byte warehousecode)
        {
            this.tankgrpname = tankgrpname;
            this.tankgrpid = tankgrpid;
            this.warehousecode = warehousecode;
        }
        public string tankgrpname { get; set; }
        public int tankgrpid { get; set; }
        public byte warehousecode { get; set; }
    }
    public class TankList
    {
        public TankList() { }
        public TankList(string tankname, int tankid)
        {
            this.tankname = tankname;
            this.tankid = tankid;
        }
        public string tankname { get; set; }
        public int tankid { get; set; }
    }
    public class ProductList
    {
        public ProductList() { }
        public ProductList(string productname, int productid)
        {
            this.productname = productname;
            this.productid = productid;
        }
        public string productname { get; set; }
        public int productid { get; set; }
    }

    public class CommandList
    {
        public CommandList() { }
        public CommandList(decimal workorder, int commandid)
        {
            this.workorder = workorder;
            this.commandid = commandid;
        }
        public decimal workorder { get; set; }
        public int commandid { get; set; }
    }

    public class Fields
    {
        public string description { get; set; }
    }

    public class Template
    {
        public string type { get; set; }
        public Fields fields { get; set; }
    }

    public class JsonObject
    {
        public List<Datum> data { get; set; }
        public string getValue { get; set; }
        public Template template { get; set; }
    }
}