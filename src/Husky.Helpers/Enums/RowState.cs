namespace Husky
{
	public enum RowState
	{
		[Label("正常")]
		Active,

		[Label("停用")]
		Inactive,

		[Label("暂停")]
		Suspended,

		[Label("用户删除")]
		DeletedByUser,

		[Label("管理删除")]
		DeletedByAdmin
	}
}
