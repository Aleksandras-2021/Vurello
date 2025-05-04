using Ardalis.Specification;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

public interface IGenericService<TEntity, TCreate, TUpdate>
    where TEntity : class
    where TCreate : class
    where TUpdate : class
{
    Task<List<TEntity>> GetAllAsync(ISpecification<TEntity> spec);
    Task<TEntity> GetSingleAsync(ISingleResultSpecification<TEntity> spec);
    Task<List<TResult>> GetAllAsync<TResult>(ISpecification<TEntity, TResult> spec);
    Task<TResult> GetSingleAsync<TResult>(ISingleResultSpecification<TEntity, TResult> spec);
    Task<TEntity> CreateAsync(TCreate create);
    Task<TEntity> UpdateAsync(Guid id, TUpdate update);
    Task DeleteAsync(Guid id);
    Task<bool> AuthorizeAsync(Guid id, ClaimsPrincipal user);
    Task<bool> AuthorizeAsync(TCreate create, ClaimsPrincipal user);
    Task<bool> AuthorizeAsync(TUpdate update, Guid id, ClaimsPrincipal user);
    Task OnCreatingAsync(TEntity entity, TCreate create);
    Task OnUpdatingAsync(TEntity entity, TUpdate update);

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

    public async Task<List<TEntity>> GetAllAsync(ISpecification<TEntity> spec)
    {
        var entities = await _repository.ListAsync(spec);

        return entities;
    }

    public async Task<TEntity> GetSingleAsync(ISingleResultSpecification<TEntity> spec)
    {
        var entity = await _repository.SingleOrDefaultAsync(spec);

        return entity;
    }
    public async Task<List<TResult>> GetAllAsync<TResult>(ISpecification<TEntity, TResult> spec)
    {
        var entities = await _repository.ListAsync(spec);
        return entities;
    }

    public async Task<TResult> GetSingleAsync<TResult>(ISingleResultSpecification<TEntity, TResult> spec)
    {
        var entity = await _repository.SingleOrDefaultAsync(spec);
        return entity;
    }

    public async Task<TEntity> CreateAsync(TCreate create)
    {
        var entity = create.Adapt<TEntity>();
        await OnCreatingAsync(entity, create);
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
        await OnUpdatingAsync(entity, update);

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


    public virtual async Task<bool> AuthorizeAsync(Guid id, ClaimsPrincipal user)
    {
        await Task.CompletedTask;

        return true;
    }

    public virtual async Task<bool> AuthorizeAsync(TCreate create, ClaimsPrincipal user)
    {
        await Task.CompletedTask;

        return true;
    }

    public virtual async Task<bool> AuthorizeAsync(TUpdate update, Guid id, ClaimsPrincipal user)
    {
        await Task.CompletedTask;

        return true;
    }
    public virtual async Task OnCreatingAsync(TEntity entity, TCreate create)
    {
        await Task.CompletedTask;
    }
    public virtual async Task OnUpdatingAsync(TEntity entity, TUpdate update)
    {
        await Task.CompletedTask;
    }
}
