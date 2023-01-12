namespace Husky.WeChatIntegration
{
	public record WxpayRefundQueryResult
	{
		public decimal RefundAmount { get; internal init; }
		public string? OriginalResult { get; internal init; }
	}
}
