using Microsoft.AspNetCore.Mvc;
using Mapster;
using PSK.Server.Specifications.Generic;
using PSK.Server.Misc;
using PSK.Server.Data.Entities;

namespace PSK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenericController<TEntity, TCreate, TUpdate> : ControllerBase
        where TEntity : class, IHasId
        where TCreate : class
        where TUpdate : class
    {
        protected readonly IGenericService<TEntity, TCreate, TUpdate> _service;

        public GenericController(IGenericService<TEntity, TCreate, TUpdate> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var entities = await _service.GetAsync(new GetAll<TEntity>());

            if (entities == null)
            {
                return NotFound();
            }

            return Ok(entities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var entity = await _service.GetAsync(new EntityByIdSpec<TEntity>(id));

            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TCreate create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.CreateAsync(create);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TUpdate update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
            var entity = await _service.GetAsync(new EntityByIdSpec<TEntity>(id));
            if (entity == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
