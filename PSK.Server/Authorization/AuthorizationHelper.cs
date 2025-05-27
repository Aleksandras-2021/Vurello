using Microsoft.EntityFrameworkCore;
using PSK.Server.Data;

namespace PSK.Server.Authorization;

public static class AuthorizationHelper
{
    public static async Task<Guid?> GetTeamIdFromRouteAsync(RouteValueDictionary routeValues, AppDbContext dbContext)
    {
        if (routeValues.TryGetValue("teamId", out var teamIdObj) && Guid.TryParse(teamIdObj?.ToString(), out var teamIdGuid))
        {
            if (await dbContext.Teams.AnyAsync(t => t.Id == teamIdGuid)) return teamIdGuid;
            return null;
        }

        if (routeValues.TryGetValue("boardId", out var boardIdObj) && Guid.TryParse(boardIdObj?.ToString(), out var boardIdGuid))
        {
            var board = await dbContext.Boards.AsNoTracking().FirstOrDefaultAsync(b => b.Id == boardIdGuid);
            if (board == null) return null;
            return board.TeamId;
        }

        if (routeValues.TryGetValue("jobId", out var jobIdObj) && Guid.TryParse(jobIdObj?.ToString(), out var jobIdGuid))
        {
            var job = await dbContext.Jobs.AsNoTracking().Include(j => j.Board).FirstOrDefaultAsync(j => j.Id == jobIdGuid);
            if (job == null || job.Board == null) return null;
            return job.Board.TeamId;
        }

        if (routeValues.TryGetValue("labelId", out var labelIdObj) && Guid.TryParse(labelIdObj?.ToString(), out var labelIdGuid))
        {
            var label = await dbContext.Labels.AsNoTracking().FirstOrDefaultAsync(l => l.Id == labelIdGuid);
            if (label == null) return null;
            return label.TeamId;
        }

        if (routeValues.TryGetValue("roleId", out var roleIdObj) && Guid.TryParse(roleIdObj?.ToString(), out var roleIdGuid))
        {
            var role = await dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == roleIdGuid);
            if (role == null) return null;
            return role.TeamId;
        }


        if (routeValues.TryGetValue("columnId", out var columnIdObj) && Guid.TryParse(columnIdObj?.ToString(), out var columnIdGuid))
        {
            var column = await dbContext.BoardColumns.AsNoTracking().Include(c => c.Board).FirstOrDefaultAsync(r => r.Id == columnIdGuid);
            if (column == null) return null;
            return column.Board.TeamId;
        }

        return null;
    }
}