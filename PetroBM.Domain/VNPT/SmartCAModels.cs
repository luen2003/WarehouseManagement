using System;
using System.Collections.Generic;

namespace PetroBM.Domain.VNPT.SmartCA
{
    #region Get Certificate
    /// <summary>
    /// Request để lấy chứng thư số từ VNPT SmartCA
    /// </summary>
    public class GetCertificateRequest
    {
        public string sp_id { get; set; }          // client_id
        public string sp_password { get; set; }    // client_secret
        public string user_id { get; set; }        // user id SmartCA
        public string serial_number { get; set; }  // serial number chứng thư
        public string transaction_id { get; set; } // mã giao dịch (unique)
    }

    /// <summary>Chuỗi chứng thư (CA, Root)</summary>
    public class CertificateChain
    {
        public string ca_cert { get; set; }
        public string root_cert { get; set; }
    }

    /// <summary>Chứng thư người dùng (trả về từ API)</summary>
    public class UserCertificate
    {
        public string service_type { get; set; }
        public string service_name { get; set; }
        public string cert_id { get; set; }
        public string cert_status { get; set; }
        public string serial_number { get; set; }
        public string cert_subject { get; set; }
        public DateTime cert_valid_from { get; set; }
        public DateTime cert_valid_to { get; set; }
        public string cert_data { get; set; }         // Base64 của cert
        public CertificateChain chain_data { get; set; }
        public string transaction_id { get; set; }
    }

    public class CertificateData
    {
        public List<UserCertificate> user_certificates { get; set; }
    }

    public class GetCertificateResponse
    {
        public int status_code { get; set; }
        public string message { get; set; }
        public CertificateData data { get; set; }
    }
    #endregion

    #region Sign
    /// <summary>File cần ký (hash)</summary>
    public class SignFile
    {
        public string data_to_be_signed { get; set; } // hex string
        public string doc_id { get; set; }
        public string file_type { get; set; } // pdf | xml
        public string sign_type { get; set; } // hash
    }

    public class SignRequest
    {
        public string sp_id { get; set; }
        public string sp_password { get; set; }
        public string user_id { get; set; }
        public string transaction_desc { get; set; }
        public string transaction_id { get; set; }
        public List<SignFile> sign_files { get; set; }
        public string serial_number { get; set; }
    }

    public class SignData
    {
        public string transaction_id { get; set; }
        public string tran_code { get; set; }
    }

    public class SignResponse
    {
        public int status_code { get; set; }
        public string message { get; set; }
        public SignData data { get; set; }
    }
    #endregion

    #region Status
    public class SignatureData
    {
        public string doc_id { get; set; }
        public string signature_value { get; set; } // base64 signature
        public string timestamp_signature { get; set; }
    }

    public class TransactionData
    {
        public string transaction_id { get; set; }
        public List<SignatureData> signatures { get; set; }
    }

    public class StatusResponse
    {
        public int status_code { get; set; }
        public string message { get; set; }
        public TransactionData data { get; set; }
    }
    #endregion
}
