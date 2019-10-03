using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ABD.Payment.Dto
{
    public class PaymentRequestDto
    {
        [JsonProperty("amount")]
        public double Amount { get; set; }
        [JsonProperty("credit_card")]
        public CreditCardDto ObjCreditCard { get; set; }
        [JsonProperty("csc")]
        public string Csc { get; set; }
        [JsonProperty("billing_address")]
        public BillingAddressDto ObjBillingAddress { get; set; }
        [JsonProperty("external_transaction_id")]
        public string ObjtransactionId { get; set; }
    }
}
