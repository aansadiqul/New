using Abp.BackgroundJobs;
using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using ABD.Entities;


namespace ABD.Hangfire
{
    public class TestJob : BackgroundJob<int>, ITransientDependency
    {
        //private readonly IRepository<Agency> _agencyRepository;
        private readonly IIocResolver _iocResolver;

        public TestJob(IIocResolver iocResolver)
        {
            //_agencyRepository = agencyRepository;
            _iocResolver = iocResolver;
        }

        public override void Execute(int number)
        {
            using (var agencyRepository = _iocResolver.ResolveAsDisposable<IRepository<Agency>>())
            {
                var result = agencyRepository.Object.GetAll().Where(x => x.GeoCodeStatus == null || x.GeoCodeStatus == 1).Take(2).ToList(); ;
            }

            //var agencies = _agencyRepository.GetAll()
            //    .Where(x => x.GeoCodeStatus == null || x.GeoCodeStatus == 1).Take(2).ToList();
            Logger.Debug("Test Job: "+number.ToString());
        }
    }
}
