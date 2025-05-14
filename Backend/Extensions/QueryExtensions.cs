using Forum.Models;
using Microsoft.EntityFrameworkCore;

namespace Forum.Extensions;

public static class QueryExtensions
{
    public static IQueryable<T> IncludeDeleted<T>(this IQueryable<T> query)
        where T : BaseModel => query.IgnoreQueryFilters();
}
