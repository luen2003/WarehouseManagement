using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using PetroBM.Web.Models;
using PetroBM.Domain.Entities;
using PetroBM.Common.Util;
using PagedList;
using PetroBM.Web.Attribute;
using System.Globalization;
using System.Web.Mvc;
using PetroBM.Services.Services;
using Microsoft.Practices.Unity;
using Unity.Mvc5;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using Microsoft.Reporting.WebForms;


namespace PetroBM.Web.Report
{
    public partial class WebForm : System.Web.UI.Page
    {

        private ICommandDetailService CommandDetailService = UnityConfig.container.Resolve<CommandDetailService>();
        public IInvoiceDetailService InvoiceDetailService= UnityConfig.container.Resolve<InvoiceDetailService>();
        public IInvoiceService InvoiceService = UnityConfig.container.Resolve<InvoiceService>();

        protected void Page_Load(object sender, EventArgs e)
        {



            if (!(IsPostBack))
            {
                //string script = string.Format("document.getElementById('{0}_Toolbar').style.display = 'none';", ReportViewer1.ClientID);
                //ClientScript.RegisterStartupScript(this.GetType(), "_Toolbar", script, true);

                if (Request.QueryString["InvoiceID"]==null)
                    return;

                int objInvoidID = int.Parse(Request.QueryString["InvoiceID"].ToString());

                int objInvoidDetailID = int.Parse(Request.QueryString["InvoicedDetailID"].ToString());  //int.Parse(Request.QueryString["InvoidDetailID"]);

                var objInvoid = InvoiceService.GetAllInvoice().Where(x => x.InvoiceID == objInvoidID).FirstOrDefault();// Dùng làm tham số
                //*********************** Tạo bảng chi cho từng ngăn **********************************************************************
                var objInvoidDetail = InvoiceDetailService.GetAllInvoiceDetail().Where(x => x.ID == objInvoidDetailID).FirstOrDefault();
                var objCommandDetail = CommandDetailService.GetAllCommandDetail().Where(x => x.CommandID == objInvoid.CommandID).ToList();

                var dt = new DataTable("Invoice");
                dt.Columns.Add("No");
                dt.Columns.Add("Content");
                var arr = objInvoidDetail.ListVolume.ToCharArray();

                for (int i = 0; i < arr.Count(); i++)
                {
                    string strContent = "";
                    for (int j = 0; j < objCommandDetail.Count(); j++)
                    {
                        if (objCommandDetail[j].CompartmentOrder.ToString() == arr[i].ToString())
                            strContent = objCommandDetail[j].TimeOrder.ToString() + " Ngăn" + objCommandDetail[j].CompartmentOrder.ToString() + "N.Độ" + objCommandDetail[j].AvgTemperature.ToString() + " Tỷ trọng:" + objCommandDetail[j].GasDensity.ToString() + "VCF: " + objCommandDetail[j].CTL_E.ToString() + "Ltt: " + objCommandDetail[j].CTL_E.ToString();
                    }
                    DataRow dr = dt.NewRow();
                    dr["No"] = i + 1;
                    dr["Content"] = strContent;
                    dt.Rows.Add(dr);
                }

                //Add tham số tại đây       
               // IEnumerable<ReportParameter> para = new List<ReportParameter>() { new ReportParameter("Address", objInvoid.Address, true), new ReportParameter("CompanyName", objInvoid.CompanyName, true), new ReportParameter("DriverName", objInvoid.DriverName, true), new ReportParameter("AccountNo", objInvoid.AccountCustomNo, true) };
               // para.Add(new ReportParameter("Address", objInvoid.Address, true));
               // para.Add(new ReportParameter("CompanyName", objInvoid.CompanyName, true));
               // para.Add(new ReportParameter("DriverName", objInvoid.DriverName, true));
               // para.Add( new ReportParameter("AccountNo", objInvoid.AccountCustomNo, true));
               // para.Add( new ReportParameter("PaymentType", objInvoid.PaymentType, true));
               // para.Add(new ReportParameter("TaxCode", objInvoid.TaxCode, true));
               //para.Add( new ReportParameter("SerialNumber", objInvoid.SerialNumber.ToString(), true));

                ReportViewer1.Reset();

                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/Invc.rdlc");

                List<ReportParameter> paramList = new List<ReportParameter>();

                paramList.Add(new ReportParameter("Address", "288"));
                paramList.Add(new ReportParameter("CompanyName", "12"));
                paramList.Add(new ReportParameter("TaxCode", "2003"));
                paramList.Add(new ReportParameter("AccountNo", "2003"));
                this.ReportViewer1.LocalReport.SetParameters(paramList);
               // this.ReportViewer1.ServerReport.SetParameters(new ReportParameter("Address", objInvoid.Address, false));
                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", dt));
                ReportViewer1.LocalReport.Refresh();

               // ShowReport(dt);





            }


        }
        void ShowReport(DataTable dt)
        {
            ReportViewer1.Reset();

            ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;

            ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/Invc.rdlc");
            ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", dt));
            ReportViewer1.LocalReport.Refresh();
        }


    }
}