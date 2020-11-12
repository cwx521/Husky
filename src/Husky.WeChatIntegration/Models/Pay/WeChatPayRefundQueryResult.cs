namespace Husky.WeChatIntegration
{
	public record WeChatPayRefundQueryResult
	{
		public decimal RefundAmount { get; internal init; }
		public string? OriginalResult { get; internal init; }
	}
}
