namespace Husky.WeChatIntegration
{
	public record WxpayRefundResult
	{
		public decimal RefundAmount { get; internal init; }
		public string? OriginalResult { get; internal init; }
	}
}
