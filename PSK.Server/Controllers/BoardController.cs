using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK.Server.Data.Entities;
using PSK.Server.Specifications.BoardSpecifications;

namespace PSK.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/board")]
    public class BoardController : GenericController<Board, BoardCreate, BoardUpdate>
    {
        private readonly IBoardService _boardService;
        public BoardController(IBoardService service) : base(service)
        {
            _boardService = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var boards = await _boardService.GetAllAsync(new GetAllBoardsSpec());
            return Ok(boards);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var board = await _boardService.GetSingleAsync(new GetBoardByIdSpec(id));
            return Ok(board);
        }

        [HttpPost("with-columns")]
        public async Task<IActionResult> CreateWithColumns([FromBody] BoardCreate create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isAuthorized = await _boardService.AuthorizeAsync(create, User);

            if (!isAuthorized)
            {
                return Forbid();
            }

            await _boardService.CreateBoardWithDefaultColumnsAsync(create);
            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public new Task<IActionResult> Create([FromBody] BoardCreate create)
        {
            return CreateWithColumns(create);
        }
    }
}