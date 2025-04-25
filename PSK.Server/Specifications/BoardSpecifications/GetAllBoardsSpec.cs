using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.BoardSpecifications
{
    public class GetAllBoardsSpec : Specification<Board>
    {
        public GetAllBoardsSpec()
        {
            Query
                .Include(b => b.Team)
                .Include(b => b.Jobs);
        }
    }
}