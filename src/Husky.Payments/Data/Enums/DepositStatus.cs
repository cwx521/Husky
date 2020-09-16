namespace Husky.Payments.Data
{
	public enum DepositStatus
	{
		[Label("等待付款")]
		AwaitPay = 0,

		[Label("已取消")]
		Cancelled = 1,

		[Label("等待确认")]
		Checking = 2,

		[Label("已自动确认")]
		CheckArrivedAuto = 1001,

		[Label("已人工确认")]
		CheckArrivedManual = 1002,

		[Label("确认失败")]
		Missing = 100001
	}
}
