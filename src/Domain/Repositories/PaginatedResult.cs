using System.Collections.Generic;

namespace Domain.Repositories
{
    public record PaginatedResult<T>(IEnumerable<T> Items, int Page, int PageSize, long TotalCount);
}
