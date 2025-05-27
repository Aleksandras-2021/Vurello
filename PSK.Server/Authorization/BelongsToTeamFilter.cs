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
    public class BelongsToTeamFilter : IAsyncAuthorizationFilter
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserContext _userContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BelongsToTeamFilter(AppDbContext dbContext, IUserContext userContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userContext = userContext;
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

            var userIsInTeam = await _dbContext.Teams
                .Where(t => t.Id == teamId.Value)
                .SelectMany(t => t.Users)
                .AnyAsync(u => u.Id == userId);

            if (!userIsInTeam)
            {
                context.Result = new ForbidResult();
                return;
            }

        }

    }
}
