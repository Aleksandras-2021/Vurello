using Ardalis.Specification;
using PSK.Server.Misc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PSK.Server.Specifications.Generic
{
    public class EntityByIdSpec<TEntity> : Specification<TEntity> where TEntity : class, IHasId
    {
        public EntityByIdSpec(Guid id)
        {
            Query.Where(e => e.Id == id);
        }
    }
}
