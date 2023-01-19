namespace Husky
{
	public enum RowStatus
	{
		[Label("正常", CssClass = "text-success")]
		Active,

		[Label("停用", CssClass = "text-warning")]
		Suspended = 2,

		[Label("已删除", CssClass = "text-danger")]
		Deleted = 9
	}
}
