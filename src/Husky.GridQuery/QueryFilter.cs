namespace Husky.GridQuery
{
	public class QueryFilter
	{
		public string Field { get; set; }
		public object Value { get; set; }
		public QueryFilterComparison Operator { get; set; }
	}
}
