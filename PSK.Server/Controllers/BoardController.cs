using Microsoft.AspNetCore.Mvc;
using PSK.Server.Data.Entities;

namespace PSK.Controllers
{

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
