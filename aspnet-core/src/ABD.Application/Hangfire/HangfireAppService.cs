using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using ABD.Common;
using ABD.Entities;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace ABD.Hangfire
{
    public class HangfireAppService : BackgroundJob<int>
    {
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IGeoLocationAppService _geoLocationJob;

        public HangfireAppService(IBackgroundJobManager backgroundJobManager, IGeoLocationAppService geoLocationJob)
        {
            _backgroundJobManager = backgroundJobManager;
            _geoLocationJob = geoLocationJob;
        }

        public override void Execute(int number)
        {
            _geoLocationJob.UpdateGeoLocation();
            Logger.Debug("Test Job: " + number.ToString());
        }

        public void TestTask()
        {

            Logger.Info("Testing Hangfire");

            //_backgroundJobManager.Enqueue<GeoLocationJob, int>(20);

            //_backgroundJobManager.Enqueue<TestJob, int>(42);
            //BackgroundJob.Schedule<GeoLocationJob>(x => x.UpdateGeoLocation(), TimeSpan.FromSeconds(10));
            // for and forgot job example
            //_backgroundJobManager.Enqueue<TestJob, int>(42);

            //// delayed job example
           // BackgroundJob.Schedule(() => Console.WriteLine("Delayed job executed"), TimeSpan.FromMinutes(1));

            //// recurring job example
            //RecurringJob.AddOrUpdate(() => Console.WriteLine("Minutely Job executed"), Cron.Minutely);

            //// Continuations job example
            //var id = BackgroundJob.Enqueue(() => Console.WriteLine("Hello, "));
            //BackgroundJob.ContinueWith(id, () => Console.WriteLine("world!"));

        }


    }
}
