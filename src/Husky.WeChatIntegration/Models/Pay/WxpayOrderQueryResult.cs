namespace Husky.WeChatIntegration
{
	public record WxpayOrderQueryResult
	{
		public string? OpenId { get; internal init; }
		public decimal Amount { get; internal init; }
		public bool HasRefund { get; internal init; }
		public string? TransactionId { get; internal init; }
		public string? OriginalResult { get; internal init; }
	}
}
