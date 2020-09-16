namespace Husky.Payments.Data
{
	public enum WithdrawalStatus
	{
		[Label("待处理")]
		New = 0,

		[Label("已取消")]
		Cancelled = 1,

		[Label("已审核")]
		Accepted = 2,

		[Label("已拒绝")]
		Rejected = 3,

		[Label("已出纳")]
		PaidOut = 1001
	}
}
