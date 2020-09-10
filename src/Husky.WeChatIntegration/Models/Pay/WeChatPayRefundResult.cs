namespace Husky.WeChatIntegration
{
	public class WeChatPayRefundResult : Result
	{
		public decimal AggregatedRefundAmount { get; internal set; }
		public string? OriginalResult { get; internal set; }
	}
}
