using Abp.Authorization;
using ABD.Authorization.Roles;
using ABD.Authorization.Users;

namespace ABD.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
