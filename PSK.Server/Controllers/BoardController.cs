using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK.Server.Data.Entities;

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

    }
}
