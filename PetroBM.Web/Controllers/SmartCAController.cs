using Newtonsoft.Json;
using PetroBM.Common.Util;
using PetroBM.Domain.VNPT.SmartCA;
using PetroBM.Services.SmartCA;
using RestSharp;
using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using System.Web.Mvc;
using VnptHashSignatures.Common;
using VnptHashSignatures.Interface;
using VnptHashSignatures.Pdf;
using VnptHashSignatures.Xml;

namespace PetroBM.Web.Controllers
{
    public class SmartCAController : Controller
    {
        private readonly SmartCAService _smartCAService;

        public SmartCAController()
        {
            // TODO: đưa 3 tham số này vào web.config/appsettings để dễ config
            var clientId = Constants.CLIENT_ID;
            var clientSecret = Constants.CLIENT_SECRET;
            var userId = "162952530";

            _smartCAService = new SmartCAService(clientId, clientSecret, userId);
        }

        /// <summary>
        /// Test: gọi API lấy thông tin chứng thư
        /// </summary>
        [HttpPost]
        public ActionResult GetCertificate()
        {
            var apiUrl = "https://gwsca.vnpt.vn/sca/sp769/v1/credentials/get_certificate";
            var cert = _smartCAService.GetCertificate(apiUrl, null);

            return Json(cert, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Test: ký 1 file PDF mẫu
        /// </summary>
        [HttpPost]
        public ActionResult Sign()
        {
            var pdfInput = Server.MapPath("~/App_Data/test.pdf");
            if (!System.IO.File.Exists(pdfInput))
            {
                return Content("Không tìm thấy file test.pdf trong App_Data.");
            }

            // 1. Lấy chứng thư số
            var cert = _smartCAService.GetCertificate(
                "https://gwsca.vnpt.vn/sca/sp769/v1/credentials/get_certificate", null);
            if (cert == null)
                return Content("Không tìm thấy chứng thư số.");

            // 2. Hash file PDF
            var unsignData = System.IO.File.ReadAllBytes(pdfInput);
            var signer = HashSignerFactory.GenerateSigner(
                unsignData, cert.cert_data, null, HashSignerFactory.PDF);
            signer.SetHashAlgorithm(MessageDigestAlgorithm.SHA256);

            var hashValue = signer.GetSecondHashAsBase64();
            var dataToSign = BitConverter
                .ToString(Convert.FromBase64String(hashValue))
                .Replace("-", "")
                .ToLower();

            // 3. Tạo transaction ký
            var signData = _smartCAService.CreateSignTransaction(
                "https://gwsca.vnpt.vn/sca/sp769/v1/signatures/sign",
                dataToSign,
                cert.serial_number);

            return Json(signData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Kiểm tra trạng thái ký từ transactionId
        /// </summary>
        [HttpPost]
        public ActionResult CheckStatus(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
                return Content("Thiếu transactionId");

            var status = _smartCAService.GetSignStatus(
                $"https://gwsca.vnpt.vn/sca/sp769/v1/signatures/sign/{transactionId}/status");

            return Json(status, JsonRequestBehavior.AllowGet);
        }
    }
}