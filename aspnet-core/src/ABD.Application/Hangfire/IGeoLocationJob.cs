using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Dependency;

namespace ABD.Hangfire
{
    public interface IGeoLocationAppService : ISingletonDependency
    {
        Task UpdateGeoLocation();
    }
}
