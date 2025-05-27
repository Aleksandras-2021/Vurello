using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK.Server.Authorization;
using PSK.Server.Data.Entities;
using PSK.Server.Specifications.BoardSpecifications;

namespace PSK.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/board")]
    public class BoardController : ControllerBase
    {
        private readonly IBoardService _boardService;
        public BoardController(IBoardService service)
        {
            _boardService = service;
        }

        [HttpGet("{boardId}")]
        [BelongsToTeam]
        public async Task<IActionResult> GetById(Guid boardId)
        {
            var board = await _boardService.GetSingleAsync(new GetBoardByIdSpec(boardId));
            return Ok(board);
        }

        [HttpPost("{teamId}/with-columns")]
        [HasPermission(PermissionName.Board)]
        public async Task<IActionResult> CreateWithColumns([FromBody] BoardCreate create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var board = await _boardService.CreateBoardWithDefaultColumnsAsync(create);
            return Ok(board);
        }

        [HttpPatch("{boardId}")]
        [HasPermission(PermissionName.Board)]
        public async Task<IActionResult> Update(Guid boardId, [FromBody] BoardUpdate update)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _boardService.UpdateAsync(boardId, update);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        [HttpDelete("{boardId}")]
        [HasPermission(PermissionName.Board)]
        public async Task<IActionResult> Delete(Guid boardId)
        {
            await _boardService.DeleteAsync(boardId);

            return NoContent();
        }
    }
}