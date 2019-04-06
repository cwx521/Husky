using Husky;

namespace Husky.GridQuery
{
	public enum QueryFilterComparison
	{
		[Label("等于")]
		Eq,
		[Label("不等于")]
		Neq,
		[Label("包含")]
		Contains,
		[Label("大于")]
		Gt,
		[Label("大于或等于")]
		Gte,
		[Label("小于")]
		Lt,
		[Label("小于或等于")]
		Lte
	}

	public static class QueryFilterComparisonEquality
	{
		public static Comparison Equality(this QueryFilterComparison comparison) {
			switch ( comparison ) {
				default:
				case QueryFilterComparison.Eq: return Comparison.Equal;
				case QueryFilterComparison.Neq: return Comparison.NotEqual;
				case QueryFilterComparison.Contains: return Comparison.HasKeyword;
				case QueryFilterComparison.Gt: return Comparison.GreaterThan;
				case QueryFilterComparison.Gte: return Comparison.GreaterThanOrEqual;
				case QueryFilterComparison.Lt: return Comparison.LessThan;
				case QueryFilterComparison.Lte: return Comparison.LessThanOrEqual;
			}
		}
	}
}