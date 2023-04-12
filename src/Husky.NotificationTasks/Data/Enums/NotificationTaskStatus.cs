namespace Husky.NotificationTasks.Data
{
	public enum NotificationTaskStatus
	{
		[Label("等待")]
		Pending,

		[Label("等待重试")]
		Retry,

		[Label("已放弃")]
		GivenUp,

		[Label("已撤销")]
		Aborted,

		[Label("已完成")]
		Completed
	}
}
