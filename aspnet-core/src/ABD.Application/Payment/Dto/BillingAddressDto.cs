using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ABD.Payment.Dto
{
    public class BillingAddressDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("street_address")]
        public string StreetAddress { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("zip")]
        public string Zip { get; set; }
    }
}
