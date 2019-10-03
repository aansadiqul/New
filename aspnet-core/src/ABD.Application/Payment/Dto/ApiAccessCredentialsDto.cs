using System;
using System.Collections.Generic;
using System.Text;

namespace ABD.Payment.Dto
{
    public class ApiAccessCredentialsDto
    {
        public static string GrantType = "password";
        public static string UserName = "transactions@neilsonmarketing.com";
        public static string Password = "NMSdata2019!";
        public static string GetFormattedRequest()
        {
            return string.Format("grant_type={0}&username={1}&password={2}", GrantType, UserName, Password);
        }
    }
}
