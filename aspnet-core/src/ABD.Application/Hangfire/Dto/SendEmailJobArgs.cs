using System;
using System.Collections.Generic;
using System.Text;

namespace ABD.Hangfire.Dto
{
    [Serializable]
    public class SendEmailJobArgs
    {
        public long SenderUserId { get; set; }

        public string TargetUser { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string AttachedFile { get; set; }
    }
}
