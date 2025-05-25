using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSK.Server.Data.Entities;
using PSK.Server.Specifications.JobSpecifications;

namespace PSK.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/job")]
    public class JobController : GenericController<Job, JobCreate, JobUpdate>
    {
        private readonly IJobService _jobService;

        public JobController(IJobService service) : base(service)
        {
            _jobService = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? boardId)
        {
            if (boardId.HasValue)
            {
                var jobs = await _jobService.GetAllAsync(new GetJobsByBoardSpec(boardId.Value));
                return Ok(jobs);
            }

            var allJobs = await _jobService.GetAllAsync(new GetAllJobsSpec());
            return Ok(allJobs);
        }

        [HttpGet("{jobId}")]
        public async Task<IActionResult> GetJobById(Guid jobId)
        {
            var job = await _jobService.GetSingleAsync(new GetJobByIdSpec(jobId));
            return Ok(job);
        }

        [HttpPut("{jobId}/labels")]
        public async Task<IActionResult> UpdateLabels(Guid jobId, [FromBody] UpdateLabels labels)
        {
            var job = await _jobService.GetSingleAsync(new GetJobByIdSpec(jobId));
            await _jobService.UpdateLabels(job, labels);

            return Ok();
        }

        [HttpPost("column-order")]
        public async Task<IActionResult> UpdateColumnOrder([FromBody] UpdateColumnOrderRequest request)
        {
            try
            {
                foreach (var jobUpdate in request.Jobs)
                {
                    var job = await _jobService.GetSingleAsync(new GetJobByIdSpec(jobUpdate.JobId));
                    if (job != null)
                    {
                        await _jobService.UpdateAsync(jobUpdate.JobId, new JobUpdate
                        {
                            ColumnPosition = jobUpdate.Position,
                            Version = job.Version
                        });
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("{jobId}/move-to-column/{columnId}")]
        public async Task<IActionResult> MoveJobToColumn(Guid jobId, Guid columnId)
        {
            try
            {
                await _jobService.MoveJobToColumnAsync(jobId, columnId);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    public class UpdateColumnOrderRequest
    {
        public List<JobPositionUpdate> Jobs { get; set; } = new List<JobPositionUpdate>();
    }

    public class JobPositionUpdate
    {
        public Guid JobId { get; set; }
        public int Position { get; set; }
    }
}