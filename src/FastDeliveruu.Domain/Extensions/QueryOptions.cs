using System.Linq.Expressions;

namespace FastDeliveruu.Domain.Extensions;

public class QueryOptions<T>
{
    private string[] includes = Array.Empty<string>();

    public string[] GetIncludes => includes;

    public string SetIncludes
    {
        set => includes = value.Replace(" ", "").Split(",");
    }

    public int PageNumber { get; set; }

    public List<Expression<Func<T, bool>>> WhereClauses { get; set; } = null!;

    public Expression<Func<T, bool>> Where
    {
        set
        {
            WhereClauses ??= new List<Expression<Func<T, bool>>>();
            WhereClauses.Add(value);
        }
    }

    public Expression<Func<T, object>> OrderBy { get; set; } = null!;

    public bool Tracked = true;

    // flags
    public bool HasInclude => includes != Array.Empty<string>();
    public bool HasPaging => PageNumber > 0;
    public bool HasWhereClause => WhereClauses != null;
    public bool HasOrderBy => OrderBy != null;
}