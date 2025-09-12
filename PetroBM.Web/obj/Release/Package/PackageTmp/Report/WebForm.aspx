<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm.aspx.cs" Inherits="PetroBM.Web.Report.WebForm" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
  <style>
        .reportViewerStyle
        {
            background-color:white;
            width:100%;
            height:100%;
            min-height:500px;
            border:none;
        }
    </style>

    <script type="text/javascript">
        function sonu(strid) {

            var printContent = document.getElementById(strid);
            var winprit = window.open('', '', 'left=0, top=0,width=1500,height=1000,toobars =0,scrollbars=0,status=0');
            winprit.document.write(printContent.innerHTML);
            winprit.document.close();
            winprit.print();
            winprit.close();
        };

        function PrintReport() {
            var viewerReference = $find("ReportViewer1");

            var stillonLoadState = clientViewer.get_isLoading();

            if (!stillonLoadState) {
                var reportArea = viewerReference.get_reportAreaContentType();
                if (reportArea == Microsoft.Reporting.WebFormsClient.ReportAreaContent.ReportPage) {
                    $find("ReportViewer1").invokePrintDialog();
                }
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:LinkButton ID="Linkbuto" runat="server" OnClientClick="sonu('divprint')">print</asp:LinkButton>
        <asp:Label ID="Label1" runat="server" Text="Số lần In"></asp:Label>

        <asp:TextBox ID="TextBox1" runat="server" Text="0"></asp:TextBox>
     <div style="text-align:center;">

            <div id ="divprint" style="width:956px;height:1500px;">
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="100%" ShowToolBar="false" AsyncRendering="false"></rsweb:ReportViewer>          
            </div>
    </div>
    </form>
</body>
</html>
