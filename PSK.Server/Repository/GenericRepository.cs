using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PSK.Server.Data;

public interface IGenericRepository<T> : IRepositoryBase<T> where T : class
{
    void UpdateVersion(T entity, uint version);
}

public class GenericRepository<T> : RepositoryBase<T>, IGenericRepository<T> where T : class
{
    private readonly AppDbContext dbContext;

    public GenericRepository(AppDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public void UpdateVersion(T entity, uint version)
    {
        var entry = dbContext.Entry(entity);
        entry.Property("Version").OriginalValue = version;
    }
}