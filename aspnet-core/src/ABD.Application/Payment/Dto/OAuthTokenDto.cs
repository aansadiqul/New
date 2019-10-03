using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace ABD.Payment.Dto
{
    public class OAuthTokenDto
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        public OAuthErrorDto ObjError { get; set; }
        public bool ErrorFlag { get; set; }
    }
}
