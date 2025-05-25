using Ardalis.Specification;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace PSK.Server.Repository;

public sealed class CachingGenericRepository<T> : IGenericRepository<T>
        where T : class
{
    private readonly IGenericRepository<T> _genericRepository;
    private readonly IMemoryCache _memoryCache;

    public CachingGenericRepository(IGenericRepository<T> genericRepository, IMemoryCache memoryCache)
    {
        _genericRepository = genericRepository ?? throw new ArgumentNullException(nameof(genericRepository));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }


    public Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
    {
        return _memoryCache.GetOrCreateAsync(
            $"Entity-{id}",
            cacheEntry =>
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return _genericRepository.GetByIdAsync(id, cancellationToken);
            });
    }

    public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        return ((IRepositoryBase<T>)_genericRepository).AddAsync(entity, cancellationToken);
    }

    public Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        return ((IRepositoryBase<T>)_genericRepository).AddRangeAsync(entities, cancellationToken);
    }

    public Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return ((IReadRepositoryBase<T>)_genericRepository).AnyAsync(specification, cancellationToken);
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return ((IReadRepositoryBase<T>)_genericRepository).AnyAsync(cancellationToken);
    }

    public IAsyncEnumerable<T> AsAsyncEnumerable(ISpecification<T> specification)
    {
        return ((IReadRepositoryBase<T>)_genericRepository).AsAsyncEnumerable(specification);
    }

    public Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return ((IReadRepositoryBase<T>)_genericRepository).CountAsync(specification, cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return ((IReadRepositoryBase<T>)_genericRepository).CountAsync(cancellationToken);
    }

    public Task<int> DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        return ((IRepositoryBase<T>)_genericRepository).DeleteAsync(entity, cancellationToken);
    }

    public Task<int> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        return ((IRepositoryBase<T>)_genericRepository).DeleteRangeAsync(entities, cancellationToken);
    }

    public Task<int> DeleteRangeAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return ((IRepositoryBase<T>)_genericRepository).DeleteRangeAsync(specification, cancellationToken);
    }

    public Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return ((IReadRepositoryBase<T>)_genericRepository).FirstOrDefaultAsync(specification, cancellationToken);
    }

    public Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        return ((IReadRepositoryBase<T>)_genericRepository).FirstOrDefaultAsync(specification, cancellationToken);
    }

    public Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        return ((IReadRepositoryBase<T>)_genericRepository).ListAsync(cancellationToken);
    }

    public Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return ((IReadRepositoryBase<T>)_genericRepository).ListAsync(specification, cancellationToken);
    }

    public Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        return ((IReadRepositoryBase<T>)_genericRepository).ListAsync(specification, cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return ((IRepositoryBase<T>)_genericRepository).SaveChangesAsync(cancellationToken);
    }

    public Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return ((IReadRepositoryBase<T>)_genericRepository).SingleOrDefaultAsync(specification, cancellationToken);
    }

    public Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        return ((IReadRepositoryBase<T>)_genericRepository).SingleOrDefaultAsync(specification, cancellationToken);
    }

    public Task<int> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var result = _genericRepository.UpdateAsync(entity, cancellationToken);

        PropertyInfo? idProp = typeof(T).GetProperty("Id");
        object? id = idProp?.GetValue(entity);

        if (id != null)
        {
            _memoryCache.Remove($"Entity-{id}");
        }

        return result;
    }

    public Task<int> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        return ((IRepositoryBase<T>)_genericRepository).UpdateRangeAsync(entities, cancellationToken);
    }

    public void UpdateVersion(T entity, uint version)
    {
        _genericRepository.UpdateVersion(entity, version);
    }
}




