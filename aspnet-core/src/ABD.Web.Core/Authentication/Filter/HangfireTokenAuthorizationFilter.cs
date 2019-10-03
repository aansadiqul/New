using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Abp;
using Abp.Authorization;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.Extensions.DependencyInjection;

namespace ABD.Authentication.Filter
{
    public class HangfireTokenAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly string _requiredPermissionName;

        public HangfireTokenAuthorizationFilter(string requiredPermissionName = null)
        {
            _requiredPermissionName = requiredPermissionName;
        }

        public bool Authorize([NotNull]DashboardContext context)
        {
            // if we have a cookies and we are in release mode
            var cookies = context.GetHttpContext().Request.Cookies;
            if (cookies["Abp.AuthToken"] != null)
            {
                var jwtCookie = cookies["Abp.AuthToken"];
                string jwtToken = jwtCookie;
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken securityToken = handler.ReadToken(jwtToken) as JwtSecurityToken;
                var id = securityToken.Claims.FirstOrDefault(claim => claim.Type.EndsWith("sub"));
                var tenanIdClaim = securityToken.Claims.FirstOrDefault(claim => claim.Type.EndsWith("tenantId"));
                int? tenanId = null;
                if (tenanIdClaim != null)
                {
                    tenanId = int.Parse(tenanIdClaim.Value);
                }
                return IsPermissionGranted(context, _requiredPermissionName, new UserIdentifier(tenanId, long.Parse(id.Value)));
            }
            return false;

        }

        private static bool IsPermissionGranted(DashboardContext context, string requiredPermissionName, UserIdentifier userIdentifier)
        {
            var permissionChecker = context.GetHttpContext().RequestServices.GetRequiredService<IPermissionChecker>();
            return permissionChecker.IsGranted(userIdentifier, requiredPermissionName);
        }
    }
}
