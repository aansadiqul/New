using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace ABD.Payment.Dto
{
    public class OAuthErrorDto
    {
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
        public string HttpTokenError { get; set; }
    }
}
