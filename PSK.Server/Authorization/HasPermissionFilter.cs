using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using PSK.Server.Data;
using PSK.Server.Data.Entities;
using PSK.Server.Misc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PSK.Server.Authorization
{
    public class HasPermissionFilter : IAsyncAuthorizationFilter
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserContext _userContext;
        private readonly PermissionName _requiredPermission;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HasPermissionFilter(AppDbContext dbContext, IUserContext userContext, PermissionName requiredPermission, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userContext = userContext;
            _requiredPermission = requiredPermission;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = _httpContextAccessor.HttpContext ?? context.HttpContext;
            if (httpContext.User == null || !httpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            Guid userId;
            try
            {
                userId = _userContext.GetUserId(httpContext.User);
            }
            catch (UnauthorizedAccessException)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            Guid? teamId = await AuthorizationHelper.GetTeamIdFromRouteAsync(httpContext.Request.RouteValues, _dbContext);

            if (!teamId.HasValue)
            {
                context.Result = new ForbidResult();
                return;
            }

            var team = await _dbContext.Teams.AsNoTracking().FirstOrDefaultAsync(t => t.Id == teamId.Value);
            if (team == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            if (team.CreatorId == userId)
            {
                return;
            }

            var userTeamRole = await _dbContext.UserTeamRoles
                .Include(utr => utr.Role)
                .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(utr => utr.UserId == userId && utr.TeamId == teamId.Value);

            if (userTeamRole == null || userTeamRole.Role == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var userPermissions = userTeamRole.Role.Permissions.Select(p => p.Name).ToList();
            var requiredPermissionNameString = Enum.GetName(typeof(PermissionName), _requiredPermission);

            if (!userPermissions.Contains(requiredPermissionNameString))
            {
                context.Result = new ForbidResult();
                return;
            }

        }
    }
}