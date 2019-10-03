using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.BackgroundJobs;
using Abp.Runtime.Session;
using ABD.Hangfire.Dto;
using Microsoft.AspNetCore.Hosting;
using MimeKit;

namespace ABD.Hangfire
{
    [AbpAuthorize]
    public class EmailAppService : ApplicationService, IEmailAppService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IHostingEnvironment _env;

        public EmailAppService(IBackgroundJobManager backgroundJobManager,
                               IHostingEnvironment env)
        {
            _backgroundJobManager = backgroundJobManager;
            _env = env;
        }

        public async Task SendEmail(SendEmailInput input)
        {
           
            await _backgroundJobManager.EnqueueAsync<SendEmailJob, SendEmailJobArgs>(
                new SendEmailJobArgs
                {
                    Subject = input.Subject,
                    Body = input.Body,
                    SenderUserId = AbpSession.GetUserId(),
                    TargetUser = input.TargetUser
                });
        }
    }
}
