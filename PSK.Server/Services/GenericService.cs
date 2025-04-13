using Ardalis.Specification;
using Mapster;

public interface IGenericService<TEntity, TCreate, TUpdate>
    where TEntity : class
    where TCreate : class
    where TUpdate : class
{
    Task<object> GetAsync(ISpecification<TEntity> spec);
    Task<TEntity> CreateAsync(TCreate create);
    Task<TEntity> UpdateAsync(Guid id, TUpdate update);
    Task DeleteAsync(Guid id);
}

public class GenericService<TEntity, TCreate, TUpdate> : IGenericService<TEntity, TCreate, TUpdate>
    where TEntity : class
    where TCreate : class
    where TUpdate : class
{
    protected readonly GenericRepository<TEntity> _repository;

    public GenericService(GenericRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public async Task<object> GetAsync(ISpecification<TEntity> spec)
    {
        var entities = await _repository.ListAsync(spec);

        if (entities.Count == 1)
        {
            return entities.First();
        }

        return entities;
    }

    public async Task<TEntity> CreateAsync(TCreate create)
    {
        var entity = create.Adapt<TEntity>();
        await _repository.AddAsync(entity);
        return entity;
    }

    public async Task<TEntity> UpdateAsync(Guid id, TUpdate update)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            throw new Exception();
        }

        update.Adapt(entity);
        await _repository.UpdateAsync(entity);

        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            throw new Exception();
        }

        await _repository.DeleteAsync(entity);
    }
}
