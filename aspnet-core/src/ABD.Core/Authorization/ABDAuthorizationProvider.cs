using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace ABD.Authorization
{
    public class ABDAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.AD, L("AD"));
            context.CreatePermission(PermissionNames.BD, L("BD"));
            context.CreatePermission(PermissionNames.Pages_UsersList, L("UsersList"));
            context.CreatePermission(PermissionNames.Pages_Admin, L("Admin"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, ABDConsts.LocalizationSourceName);
        }
    }
}
