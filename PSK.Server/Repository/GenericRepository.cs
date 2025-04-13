using Ardalis.Specification.EntityFrameworkCore;
using PSK.Server.Data;

public class GenericRepository<T> : RepositoryBase<T> where T : class
{
    private readonly AppDbContext dbContext;

    public GenericRepository(AppDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

}
