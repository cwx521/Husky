namespace Husky.WeChatIntegration
{
	public class WeChatPayRefundQueryResult : Result
	{
		public decimal RefundAmount { get; internal set; }
		public string? OriginalResult { get; internal set; }
	}
}
