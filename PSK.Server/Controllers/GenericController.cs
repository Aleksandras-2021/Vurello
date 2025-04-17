using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PSK.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GenericController<TEntity, TCreate, TUpdate> : ControllerBase
        where TEntity : class
        where TCreate : class
        where TUpdate : class
    {
        protected readonly IGenericService<TEntity, TCreate, TUpdate> _service;

        public GenericController(IGenericService<TEntity, TCreate, TUpdate> service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TCreate create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isAuthorized = await _service.AuthorizeAsync(create, User);

            if (!isAuthorized)
            {
                return Forbid();
            }

            await _service.CreateAsync(create);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public virtual async Task<IActionResult> Update(Guid id, [FromBody] TUpdate update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isAuthorized = await _service.AuthorizeAsync(update, id, User);

            if (!isAuthorized)
            {
                return Forbid();
            }

            var updated = await _service.UpdateAsync(id, update);
            if (updated == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var isAuthorized = await _service.AuthorizeAsync(id, User);

            if (!isAuthorized)
            {
                return Forbid();
            }

            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}
