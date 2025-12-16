using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TicketMaster.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // Admin users bypass all permission checks
            var isAdmin = context.User.FindFirst("IsAdmin")?.Value == "True";
            if (isAdmin)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Check if user has the required permission
            var hasPermission = context.User.Claims
                .Any(c => c.Type == "Permission" && c.Value == requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
