using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;

namespace ABD.Hangfire
{
    public interface IHangfireAppService : IApplicationService
    {
        void TestTask();
    }
}
