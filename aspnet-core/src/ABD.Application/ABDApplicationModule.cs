using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using ABD.Authorization;

namespace ABD
{
    [DependsOn(
        typeof(ABDCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class ABDApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<ABDAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(ABDApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddProfiles(thisAssembly)
            );
        }
    }
}
