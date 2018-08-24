namespace Husky
{
	public enum Comparison
	{
		[Label("等于")]
		Equal,

		[Label("不等于")]
		NotEqual,

		[Label("包含")]
		HasKeyword,

		[Label("大于")]
		GreaterThan,

		[Label("大于或等于")]
		GreaterThanOrEqual,

		[Label("小于")]
		LessThan,

		[Label("小于或等于")]
		LessThanOrEqual
	}
}
