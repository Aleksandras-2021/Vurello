using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.BoardSpecifications
{
    public class GetBoardByIdSpec : Specification<Board>, ISingleResultSpecification<Board>
    {
        public GetBoardByIdSpec(Guid boardId)
        {
            Query
                .Where(b => b.Id == boardId)
                .Include(b => b.Team) 
                .Include(b => b.Jobs); 
        }
    }
}