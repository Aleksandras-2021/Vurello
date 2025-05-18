using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSK.Controllers;
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
    }
}