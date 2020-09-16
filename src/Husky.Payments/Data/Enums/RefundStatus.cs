namespace Husky.Payments.Data
{
	public enum RefundStatus
	{
		[Label("新申请")]
		New = 0,

		[Label("已取消")]
		Cancelled = 1,

		[Label("等待确认")]
		Checking = 2,

		[Label("已自动退款")]
		RefundedAuto = 1001,

		[Label("已人工退款")]
		RefundedManual = 1002,

		[Label("退款失败")]
		Failed = 100001
	}
}
