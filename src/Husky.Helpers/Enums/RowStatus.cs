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

		[Label("用户删除", CssClass = "text-danger")]
		DeletedByUser,

		[Label("管理删除", CssClass = "text-danger")]
		DeletedByAdmin
	}
}
