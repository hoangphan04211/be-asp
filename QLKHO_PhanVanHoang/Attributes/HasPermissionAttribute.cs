using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;
using QLKHO_PhanVanHoang.Constants;

namespace QLKHO_PhanVanHoang.Attributes
{
    public class HasPermissionAttribute : TypeFilterAttribute
    {
        public HasPermissionAttribute(string permission) : base(typeof(HasPermissionFilter))
        {
            Arguments = new object[] { permission };
        }
    }

    public class HasPermissionFilter : IAuthorizationFilter
    {
        private readonly string _permission;

        public HasPermissionFilter(string permission)
        {
            _permission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Allow if user is not authenticated yet (handled by [Authorize])
            if (context.HttpContext.User.Identity == null || !context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Admin always has all permissions
            if (context.HttpContext.User.IsInRole(AppRoles.Admin))
            {
                return;
            }

            // Check if user has the required permission in their claims
            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == "Permission" && c.Value == _permission);
            
            if (!hasClaim)
            {
                context.Result = new ForbidResult(); // Returns 403 Forbidden
            }
        }
    }
}
