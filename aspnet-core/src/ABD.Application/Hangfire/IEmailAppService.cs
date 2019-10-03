using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using ABD.Hangfire.Dto;

namespace ABD.Hangfire
{
    internal interface IEmailAppService : IApplicationService
    {
        Task SendEmail(SendEmailInput input);
    }
}
