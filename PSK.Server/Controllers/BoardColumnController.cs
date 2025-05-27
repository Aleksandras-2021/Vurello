using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK.Server.Authorization;
using PSK.Server.Data.Entities;
using System.Collections.Concurrent;

namespace PSK.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/board-column")]
    public class BoardColumnController : ControllerBase
    {
        private readonly IBoardColumnService _columnService;
        private static readonly ConcurrentDictionary<Guid, bool> _initializedBoards = new ConcurrentDictionary<Guid, bool>();

        public BoardColumnController(IBoardColumnService service)
        {
            _columnService = service;
        }

        [HttpGet("board/{boardId}")]
        [BelongsToTeam]
        public async Task<IActionResult> GetBoardColumns(Guid boardId)
        {
            var columns = await _columnService.GetBoardColumnsAsync(boardId);
            var needsDefaults = columns.Count == 0;

            if (needsDefaults && !_initializedBoards.ContainsKey(boardId))
            {
                if (_initializedBoards.TryAdd(boardId, true))
                {
                    await _columnService.EnsureDefaultColumnsExistAsync(boardId);
                    columns = await _columnService.GetBoardColumnsAsync(boardId);
                }
            }

            var uniqueColumns = columns
                .GroupBy(c => new { c.Name, c.IsDefault })
                .Select(g => g.OrderBy(c => c.Id).First())
                .OrderBy(c => c.Order)
                .ToList();

            return Ok(uniqueColumns);
        }

        [HttpPost("{boardId}/reorder")]
        [HasPermission(PermissionName.Board)]
        public async Task<IActionResult> ReorderColumns([FromBody] ReorderColumnsRequest request)
        {
            await _columnService.ReorderColumnsAsync(request.BoardId, request.ColumnIds);
            return Ok();
        }


        [HttpDelete("column/{columnId}")]
        [HasPermission(PermissionName.Board)]
        public async Task<IActionResult> DeleteColumn(Guid columnId, [FromQuery] Guid? targetColumnId = null)
        {
            await _columnService.DeleteColumnAsync(columnId, targetColumnId);
            return NoContent();
        }

        [HttpPost("{boardId}")]
        [HasPermission(PermissionName.Board)]
        public async Task<IActionResult> Create([FromBody] BoardColumnCreate create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var board = await _columnService.CreateAsync(create);
            return Ok(board);
        }

        [HttpPatch("{boardId}")]
        [HasPermission(PermissionName.Board)]
        public async Task<IActionResult> Update(Guid boardId, [FromBody] BoardColumnUpdate update)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _columnService.UpdateAsync(boardId, update);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

    }

    public class ReorderColumnsRequest
    {
        public Guid BoardId { get; set; }
        public List<Guid> ColumnIds { get; set; } = new List<Guid>();
    }
}