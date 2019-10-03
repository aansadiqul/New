using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace ABD.Controllers
{
    public abstract class ABDControllerBase: AbpController
    {
        protected ABDControllerBase()
        {
            LocalizationSourceName = ABDConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
