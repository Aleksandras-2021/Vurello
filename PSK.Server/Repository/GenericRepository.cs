using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PSK.Server.Data;

public class GenericRepository<T> : RepositoryBase<T> where T : class
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
