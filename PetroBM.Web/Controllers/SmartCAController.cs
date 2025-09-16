using PetroBM.Common.Util;
using PetroBM.Services.Services;
using PetroBM.Services.SmartCA;
using System;
using System.Runtime.ConstrainedExecution;
using System.Web.Mvc;
using VnptHashSignatures.Common;
using VnptHashSignatures.Interface;

namespace PetroBM.Web.Controllers
{
    public class SmartCAController : Controller
    {
        private readonly ISmartCAService smartCAService;
        private readonly IUserService userService;

        public SmartCAController(IUserService userService)
        {
            this.userService = userService;
        }

        // Route: /SmartCA/Test
        public ActionResult Test()
        {
            var clientId = Constants.CLIENT_ID;
            var clientSecret = Constants.CLIENT_SECRET;

            // Lấy username tại đây, khi HttpContext đã tồn tại
            var userName = HttpContext.User.Identity.Name;

            var userId = userService.GetUserIDByUserName(userName);
            var serialNumber = userService.GetSerialNumberByUserName(userName);

            var smartCAService = new SmartCAService(clientId, clientSecret, userId, serialNumber);

            // Lấy transactionId từ Session, nếu chưa có thì tạo mới
            var transactionId = Session["TransactionId"] as string;
            if (string.IsNullOrEmpty(transactionId))
            {
                transactionId = TransactionIdUtil.Generate();
                Session["TransactionId"] = transactionId;
            }

            var certData = smartCAService.GetCertificateData(transactionId);

            return Json(new
            {
                transactionId,
                certData,
                userId,
                serialNumber
            }, JsonRequestBehavior.AllowGet);
        }

        // Route: /SmartCA/CheckCertificate
        public ActionResult CheckCertificate()
        {
            // Lấy username hiện tại
            var userName = HttpContext.User.Identity.Name;

            // Lấy thông tin user
            var userId = userService.GetUserIDByUserName(userName);
            var serialNumber = userService.GetSerialNumberByUserName(userName);

            // Tạo SmartCAService cho user này
            var clientId = Constants.CLIENT_ID;
            var clientSecret = Constants.CLIENT_SECRET;
            var smartCAService = new SmartCAService(clientId, clientSecret, userId, serialNumber);

            // Tạo transactionId và lưu vào Session
            var transactionId = TransactionIdUtil.Generate();
            Session["TransactionId"] = transactionId;

            // Gọi đến Service để lấy certData
            var certData = smartCAService.GetCertificateData(transactionId);

            // Trả về JSON
            return Json(new
            {
                userId,
                serialNumber,
                transactionId,
                certData
            }, JsonRequestBehavior.AllowGet);
        }

        // Route: /SmartCA/Sign
        public ActionResult Sign()
        {
            // Lấy hoặc tạo transactionId
            var transactionId = Session["TransactionId"] as string;
            if (string.IsNullOrEmpty(transactionId))
            {
                transactionId = TransactionIdUtil.Generate();
                Session["TransactionId"] = transactionId;
            }

            // Lấy username hiện tại
            var userName = HttpContext.User.Identity.Name;

            // Lấy userId và serialNumber
            var userId = userService.GetUserIDByUserName(userName);
            var serialNumber = userService.GetSerialNumberByUserName(userName);

            // Tạo SmartCAService
            var clientId = Constants.CLIENT_ID;
            var clientSecret = Constants.CLIENT_SECRET;
            var smartCAService = new SmartCAService(clientId, clientSecret, userId, serialNumber);

            // Lấy certData (giống như gọi Check())
            var certData = smartCAService.GetCertificateData(transactionId);
            if (string.IsNullOrEmpty(certData))
            {
                return Json(new
                {
                    success = false,
                    message = "Không lấy được certData từ VNPT."
                }, JsonRequestBehavior.AllowGet);
            }

            var pdf = Server.MapPath("~/App_Data/dispatch.pdf");
            if(!System.IO.File.Exists(pdf))
            {
                return Json(new
                {
                    success = false,
                    message = "File PDF để ký không tồn tại."
                }, JsonRequestBehavior.AllowGet);
            }

            // Tính hash của file PDF
            var unsignData = System.IO.File.ReadAllBytes(pdf);
            var signer = HashSignerFactory.GenerateSigner(
                unsignData, certData, null, HashSignerFactory.PDF);
            signer.SetHashAlgorithm(MessageDigestAlgorithm.SHA256);

            var hashValue = signer.GetSecondHashAsBase64();
            var dataToSign = BitConverter
                .ToString(Convert.FromBase64String(hashValue))
                .Replace("-", "")
                .ToLower();

            // Gọi CreateSignTransaction để ký
            var signResult = smartCAService.CreateSignTransaction(transactionId, dataToSign);
            if (signResult == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Ký số thất bại."
                }, JsonRequestBehavior.AllowGet);
            }

            // Trả kết quả
            return Json(new
            {
                success = true,
                transactionId = transactionId,
                tranCode = signResult.tran_code
            }, JsonRequestBehavior.AllowGet);
        }

        // Route: /SmartCA/CheckStatus
        public ActionResult CheckStatus()
        {
            // Lấy hoặc tạo transactionId
            var transactionId = Session["TransactionId"] as string;
            //if (string.IsNullOrEmpty(transactionId))
            //{
            //    transactionId = TransactionIdUtil.Generate();
            //    Session["TransactionId"] = transactionId;
            //}

            // Lấy username hiện tại
            var userName = HttpContext.User.Identity.Name;

            // Lấy userId và serialNumber
            var userId = userService.GetUserIDByUserName(userName);
            var serialNumber = userService.GetSerialNumberByUserName(userName);

            // Tạo SmartCAService
            var clientId = Constants.CLIENT_ID;
            var clientSecret = Constants.CLIENT_SECRET;
            var smartCAService = new SmartCAService(clientId, clientSecret, userId, serialNumber);

            // Gọi GetSignStatus để kiểm tra trạng thái ký
            var statusResult = smartCAService.GetSignStatus(transactionId);
            if (statusResult == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Không lấy được trạng thái ký số."
                }, JsonRequestBehavior.AllowGet);
            }

            // Nếu trạng thái ký đang chờ, có thể thêm logic xử lý ở đây
            if (statusResult.message == "PENDING")
            {
                
            }

            // Nếu trạng thái ký bị từ chối, có thể thêm logic xử lý ở đây
            if (statusResult.message == "REJECTED")
            {
                // Xử lý khi ký bị từ chối
            }

            // Nếu trạng thái ký thành công, xóa transactionId trong Session
            if (statusResult.message == "SUCCESS")
            {
                Session["TransactionId"] = null;
            }

            // Trả kết quả
            return Json(new
            {
                success = true,
                //transactionId = transactionId,
                statusResult
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
