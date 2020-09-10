namespace Husky.WeChatIntegration
{
	public class WeChatPayRefundQueryResult : Result
	{
		public string? RefundReason { get; internal set; }
		public decimal RefundAmount { get; internal set; }
		public string? OriginalResult { get; internal set; }
	}
}
