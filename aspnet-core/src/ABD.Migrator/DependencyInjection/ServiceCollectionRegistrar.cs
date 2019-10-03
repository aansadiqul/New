using Microsoft.Extensions.DependencyInjection;
using Castle.Windsor.MsDependencyInjection;
using Abp.Dependency;
using ABD.EntityFrameworkCore;
using ABD.Identity;
using Castle.MicroKernel.Registration;
using Microsoft.EntityFrameworkCore;

namespace ABD.Migrator.DependencyInjection
{
    public static class ServiceCollectionRegistrar
    {
        public static void Register(IIocManager iocManager)
        {
            var services = new ServiceCollection();

            IdentityRegistrar.Register(services);

            WindsorRegistrationHelper.CreateServiceProvider(iocManager.IocContainer, services);

            var builder = new DbContextOptionsBuilder<ABDDbContext>();
            iocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<ABDDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );
        }
    }
}
