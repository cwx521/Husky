namespace Husky.WeChatIntegration
{
	public record WeChatPayRefundResult
	{
		public decimal RefundAmount { get; internal init; }
		public string? OriginalResult { get; internal init; }
	}
}
