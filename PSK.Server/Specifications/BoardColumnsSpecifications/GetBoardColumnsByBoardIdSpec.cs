using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.BoardColumnSpecifications
{
    public class GetBoardColumnsByBoardIdSpec : Specification<BoardColumn>
    {
        public GetBoardColumnsByBoardIdSpec(Guid boardId)
        {
            Query.Where(column => column.BoardId == boardId);
        }
    }

    public class GetBoardColumnByIdSpec : Specification<BoardColumn>, ISingleResultSpecification<BoardColumn>
    {
        public GetBoardColumnByIdSpec(Guid id)
        {
            Query.Where(column => column.Id == id);
        }
    }
}