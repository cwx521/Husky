namespace Husky.GridQuery
{
	public class QueryFilter
	{
		public string Field { get; set; } = null!;
		public object Value { get; set; } = null!;
		public QueryFilterComparison Operator { get; set; }
	}
}
