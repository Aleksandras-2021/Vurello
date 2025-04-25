using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    }
}