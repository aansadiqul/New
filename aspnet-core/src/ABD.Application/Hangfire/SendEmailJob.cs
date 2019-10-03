using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Net.Mail;
using ABD.Authorization.Users;
using ABD.Domain.Dtos;
using ABD.Hangfire.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ABD.Hangfire
{
    public class SendEmailJob : BackgroundJob<SendEmailJobArgs>, ISingletonDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly IHostingEnvironment _env;
        private readonly SmtpSettings _smtpSettings;


        public SendEmailJob(IEmailSender emailSender,
                            IHostingEnvironment env,
            IOptions<SmtpSettings> smtpSettings)
        {
            _emailSender = emailSender;
            _env = env;
            _smtpSettings = smtpSettings.Value;
        }

        public override void Execute(SendEmailJobArgs args)
        {
            MailMessage mail = new MailMessage
            {
                To = { _smtpSettings.FromAddress },
                From = new MailAddress("emailer@neilsonmarketing.com", "Neilson Marketing"),
                Subject = args.Subject,
                Body = args.Body,
                IsBodyHtml = true
            };

            if (!string.IsNullOrEmpty(args.AttachedFile))
            {
                mail.Attachments.Add(new Attachment(_env.WebRootPath + Path.DirectorySeparatorChar
                                                                     + "Orders"
                                                                     + Path.DirectorySeparatorChar
                                                                     + args.AttachedFile));
            }
            
            _emailSender.SendAsync(mail);
        } 
    }
}
