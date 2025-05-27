using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK.Server.Data.Entities.Comment;
using PSK.Server.Services;
using PSK.Server.Misc;
using PSK.Server.Authorization;

namespace PSK.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/comments")]
    public class UserCommentController : ControllerBase
    {
        private readonly IUserCommentService _commentService;
        private readonly IUserContext _userContext;

        public UserCommentController(IUserCommentService commentService, IUserContext userContext)
        {
            _commentService = commentService;
            _userContext = userContext;
        }

        [HttpGet("job/{jobId}")]
        [BelongsToTeam]
        public async Task<IActionResult> GetCommentsForJob(Guid jobId)
        {
            var comments = await _commentService.GetCommentsByJobIdAsync(jobId);
            return Ok(comments);
        }

        [HttpPost("{jobId}")]
        [BelongsToTeam]
        public async Task<IActionResult> CreateComment([FromBody] UserCommentCreate commentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _userContext.GetUserId(User);
            commentDto.CreatorId = userId;

            var createdComment = await _commentService.CreateCommentAsync(commentDto);
            return Ok(createdComment);
        }
    }
}
