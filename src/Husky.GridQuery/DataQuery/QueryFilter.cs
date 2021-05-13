namespace Husky.GridQuery
{
	public class QueryFilter
	{
		public string Field { get; set; } = null!;
		public string Value { get; set; } = null!;
		public QueryFilterComparison Operator { get; set; }
	}
}
