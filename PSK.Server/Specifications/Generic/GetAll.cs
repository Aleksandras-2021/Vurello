using Ardalis.Specification;
using PSK.Server.Misc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PSK.Server.Specifications.Generic
{
    public class GetAll<TEntity> : Specification<TEntity> where TEntity : class
    {
        public GetAll()
        {

        }
    }
}
