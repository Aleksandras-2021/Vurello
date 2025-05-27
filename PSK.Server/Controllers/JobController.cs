using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSK.Server.Authorization;
using PSK.Server.Data.Entities;
using PSK.Server.Specifications.JobSpecifications;

namespace PSK.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/job")]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobController(IJobService service)
        {
            _jobService = service;
        }

        [HttpGet("board/{boardId}")]
        [BelongsToTeam]
        public async Task<IActionResult> GetAll(Guid boardId)
        {
            var jobs = await _jobService.GetAllAsync(new GetJobsByBoardSpec(boardId));
            return Ok(jobs);
        }

        [HttpGet("{jobId}")]
        [BelongsToTeam]
        public async Task<IActionResult> GetJobById(Guid jobId)
        {
            var job = await _jobService.GetSingleAsync(new GetJobByIdSpec(jobId));
            return Ok(job);
        }

        [HttpPut("{jobId}/labels")]
        [HasPermission(PermissionName.Job)]
        public async Task<IActionResult> UpdateLabels(Guid jobId, [FromBody] UpdateLabels labels)
        {
            var job = await _jobService.GetSingleAsync(new GetJobByIdSpec(jobId));
            
            if (job == null)
            {
                return NotFound();
            }

            await _jobService.UpdateLabels(job, labels);

            return Ok(job);
        }

        [HttpPost("{jobId}/move-to-column/{columnId}")]
        [HasPermission(PermissionName.Job)]
        public async Task<IActionResult> MoveJobToColumn(Guid jobId, Guid columnId)
        {
                await _jobService.MoveJobToColumnAsync(jobId, columnId);
                return Ok();
        }

        [HttpPost("{jobId}/move-to-board/{targetBoardId}")]
        [HasPermission(PermissionName.Job)]
        public async Task<IActionResult> MoveJobToBoard(Guid jobId, Guid targetBoardId)
        {

            await _jobService.MoveJobToBoardAsync(jobId, targetBoardId);
            return Ok(new { message = "Job moved to new board successfully" });
        }

        [HttpPost("{teamId}")]
        [HasPermission(PermissionName.Job)]
        public async Task<IActionResult> Create([FromBody] JobCreate create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _jobService.CreateAsync(create);
            return Ok(entity);
        }

        [HttpPatch("{jobId}")]
        [HasPermission(PermissionName.Job)]
        public async Task<IActionResult> Update(Guid jobId, [FromBody] JobUpdate update)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _jobService.UpdateAsync(jobId, update);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        [HttpDelete("{jobId}")]
        [HasPermission(PermissionName.Job)]
        public async Task<IActionResult> Delete(Guid jobId)
        {
            await _jobService.DeleteAsync(jobId);

            return NoContent();
        }
    }

}