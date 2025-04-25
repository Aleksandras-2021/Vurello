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
    }

}
