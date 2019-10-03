using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ABD.Payment.Dto
{
    public class TempResponse
    {
        public string JsonResponse { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class PayTraceBasicResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("response_code")]
        public int ResponseCode { get; set; }
        [JsonProperty("status_message")]
        public string StatusMessage { get; set; }
        [JsonProperty("transaction_id")]
        public long TransactionId { get; set; }
        public string HttpErrorMessage { get; set; }
        [JsonProperty("errors")]
        public Dictionary<string, string[]> TransactionErrors { get; set; }
    }
    public class PayTraceExternalTransResponse : PayTraceBasicResponse
    {
        [JsonProperty("external_transaction_id")]
        public string ExternalTransactionId { get; set; }
    }
    public class PayTraceBasicSaleResponse : PayTraceExternalTransResponse
    {
        [JsonProperty("approval_code")]
        public string ApprovalCode { get; set; }
        [JsonProperty("approval_message")]
        public string ApprovalMessage { get; set; }
        [JsonProperty("avs_response")]
        public string AvsResponse { get; set; }
        [JsonProperty("csc_response")]
        public string CscResponse { get; set; }
    }
    public class KeyedSaleResponse : PayTraceBasicSaleResponse
    {
        [JsonProperty("masked_card_number")]
        public string MaskedCardNumber { get; set; }
    }

}
