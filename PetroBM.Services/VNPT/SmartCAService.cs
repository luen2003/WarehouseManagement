using Newtonsoft.Json;
using PetroBM.Common.Util;
using PetroBM.Domain.VNPT.SmartCA;
using RestSharp;
using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Threading;

namespace PetroBM.Services.SmartCA
{
    public interface ISmartCAService
    {
        /// <summary>
        /// Lấy chứng thư số (chỉ lấy cert_data)
        /// </summary>
        /// <returns>Chuỗi cert_data (Base64) hoặc null nếu lỗi</returns>
        string GetCertificateData(string transactionId);

        /// <summary>
        /// Tao giao dịch ký số
        /// </summary>
        SignData CreateSignTransaction(string transactionId, string dataToBeSigned);

        /// <summary>
        /// Kiểm tra trạng thái ký số
        /// </summary>
        StatusResponse GetSignStatus(string transactionId);
    }

    public class SmartCAService : ISmartCAService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _userId;
        private readonly string _serialNumber;
        private readonly string _baseUrl = "https://gwsca.vnpt.vn/sca/sp769/v1";

        public SmartCAService(string clientId, string clientSecret, string userId, string serialNumber)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _userId = userId;
            _serialNumber = serialNumber;
        }

        #region Helper: Post
        private string Post(string serviceUri, object body)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((sender, cert, chain, errors) => true);

            var client = new RestClient(serviceUri);
            var request = new RestRequest(serviceUri, Method.Post);
            request.AddHeader("Content-Type", "application/json");

            var jsonBody = JsonConvert.SerializeObject(body);
            request.AddStringBody(jsonBody, DataFormat.Json);

            try
            {
                var response = client.Execute(request);
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
                return response.Content;
            }
            catch
            {
                return null;
            }
        }
        #endregion


        public string GetCertificateData(string transactionId)
        {
            var requestData = new GetCertificateRequest
            {
                sp_id = _clientId,
                sp_password = _clientSecret,
                user_id = _userId,
                serial_number = _serialNumber,
                transaction_id = transactionId
            };

            var url = $"{_baseUrl}/credentials/get_certificate";
            var responseJson = Post(url, requestData);
            if (responseJson == null) return null;

            var response = JsonConvert.DeserializeObject<GetCertificateResponse>(responseJson);
            //return response?.data?.user_certificates?[0]?.cert_data;
            return response?.data?.user_certificates?.FirstOrDefault()?.cert_data;
        }

        public SignData CreateSignTransaction(string transactionId, string dataToBeSigned)
        {
            var signFile = new SignFile
            {
                data_to_be_signed = dataToBeSigned,
                doc_id = Guid.NewGuid().ToString(),
                file_type = "pdf",
                sign_type = "hash"
            };

            var requestData = new SignRequest
            {
                sp_id = _clientId,
                sp_password = _clientSecret,
                user_id = _userId,
                serial_number = _serialNumber,
                transaction_id = transactionId,
                transaction_desc = "Ký số TKV",
                sign_files = new System.Collections.Generic.List<SignFile> { signFile }
            };

            var url = $"{_baseUrl}/signatures/sign";

            var responseJson = Post(url, requestData);
            if (string.IsNullOrEmpty(responseJson)) return null;

            try
            {
                var response = JsonConvert.DeserializeObject<SignResponse>(responseJson);
                return response?.data;
            }
            catch
            {
                return null;
            }
        }

        public StatusResponse GetSignStatus(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
            {
                throw new ArgumentException("transactionId không được null hoặc rỗng");
            }

            var url = $"{_baseUrl}/signatures/sign/{transactionId}/status";

            var responseJson = Post(url, new { });
            if (string.IsNullOrEmpty(responseJson)) return null;
            try
            {
                var response = JsonConvert.DeserializeObject<StatusResponse>(responseJson);
                return response;
            }
            catch
            {
                return null;
            }
        }
    }
}