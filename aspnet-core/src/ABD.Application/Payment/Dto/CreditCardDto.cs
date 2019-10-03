using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ABD.Payment.Dto
{
    public class CreditCardDto
    {
        [JsonProperty("number")]
        public string CcNumber { get; set; }
        [JsonProperty("expiration_month")]
        public string ExpirationMonth { get; set; }
        [JsonProperty("expiration_year")]
        public string ExpirationYear { get; set; }
    }
}
