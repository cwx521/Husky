namespace Husky.WeChatIntegration
{
	public record WxpayNotifyResult
	{
		public string OrderNo { get; internal init; } = null!;
		public string TransactionId { get; internal init; } = null!;
		public string OpenId { get; internal init; } = null!;
		public decimal Amount { get; internal init; }
		public string? Attach { get; internal init; }
		public string? OriginalResult { get; internal init; }
	}
}
