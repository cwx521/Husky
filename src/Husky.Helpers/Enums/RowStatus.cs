namespace Husky
{
	public enum RowStatus
	{
		[Label("正常", CssClass = "text-success")]
		Active,

		[Label("非活动", CssClass = "text-muted")]
		Inactive,

		[Label("停用", CssClass = "text-warning")]
		Suspended,

		[Label("已删除", CssClass = "text-danger")]
		Deleted = 9
	}
}
