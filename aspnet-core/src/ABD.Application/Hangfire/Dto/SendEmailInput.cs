using System;
using System.Collections.Generic;
using System.Text;

namespace ABD.Hangfire.Dto
{
    public class SendEmailInput
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string TargetUser { get; set; }
    }
}
