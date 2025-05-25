using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK.Controllers;
using PSK.Server.Data.Entities;
using PSK.Server.Misc;
using PSK.Server.Services;
using PSK.Server.Specifications.LabelSpecifications;

namespace PSK.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/label")]
    public class LabelController : GenericController<Label, LabelCreate, LabelUpdate>
    {
        private readonly ILabelService _labelService;
        private readonly IUserContext _userContext;
        public LabelController(ILabelService service, IUserContext userContext) : base(service)
        {
            _labelService = service;
            _userContext = userContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var labels = await _labelService.GetAllAsync(new GetAllLabelsSpec());
            return Ok(labels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var label = await _labelService.GetSingleAsync(new GetLabelByIdWithJobsSpec(id));
            return Ok(label);
        }

        [HttpGet("team/{teamId}")]
        public async Task<IActionResult> GetTeamLabels(Guid teamId)
        {
            var labels = await _labelService.GetAllAsync(new GetLabelsByTeamIdSpec(teamId));
            return Ok(labels);
        }
    }
}
