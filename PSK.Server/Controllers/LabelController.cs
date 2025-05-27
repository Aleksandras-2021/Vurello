using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK.Controllers;
using PSK.Server.Authorization;
using PSK.Server.Data.Entities;
using PSK.Server.Misc;
using PSK.Server.Services;
using PSK.Server.Specifications.LabelSpecifications;

namespace PSK.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/label")]
    public class LabelController : ControllerBase
    {
        private readonly ILabelService _labelService;
        private readonly IUserContext _userContext;
        public LabelController(ILabelService service, IUserContext userContext) 
        {
            _labelService = service;
            _userContext = userContext;
        }

        [HttpGet("team/{teamId}")]
        [BelongsToTeam]
        public async Task<IActionResult> GetTeamLabels(Guid teamId)
        {
            var labels = await _labelService.GetAllAsync(new GetLabelsByTeamIdSpec(teamId));
            return Ok(labels);
        }

        [HttpGet("{labelId}")]
        [BelongsToTeam]
        public async Task<IActionResult> GetById(Guid labelId)
        {
            var label = await _labelService.GetSingleAsync(new GetLabelByIdWithJobsSpec(labelId));
            return Ok(label);
        }

        [HttpPost("{teamId}")]
        [HasPermission(PermissionName.Labels)]
        public async Task<IActionResult> Create([FromBody] LabelCreate create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _labelService.CreateAsync(create);
            return Ok(entity);
        }

        [HttpPatch("{labelId}")]
        [HasPermission(PermissionName.Labels)]
        public async Task<IActionResult> Update(Guid labelId, [FromBody] LabelUpdate update)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _labelService.UpdateAsync(labelId, update);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        [HttpDelete("{labelId}")]
        [HasPermission(PermissionName.Labels)]
        public async Task<IActionResult> Delete(Guid labelId)
        {
            await _labelService.DeleteAsync(labelId);

            return NoContent();
        }
    }
}
