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
			return comparison switch {
				QueryFilterComparison.Neq => Comparison.NotEqual,
				QueryFilterComparison.Contains => Comparison.HasKeyword,
				QueryFilterComparison.Gt => Comparison.GreaterThan,
				QueryFilterComparison.Gte => Comparison.GreaterThanOrEqual,
				QueryFilterComparison.Lt => Comparison.LessThan,
				QueryFilterComparison.Lte => Comparison.LessThanOrEqual,
				_ => Comparison.Equal,
			};
		}
	}
}