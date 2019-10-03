using System;
using System.Collections.Generic;
using System.Text;

namespace ABD.Domain.Dtos
{
    public class SmtpSettings
    {
        public string Host { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public string Port { get; set; }
        public string FromAddress { get; set; }
        public string DisplayName { get; set; }
        public string EnableSsl { get; set; }
        public string UseDefaultCredentials { get; set; }
    }
}
