namespace Husky.WeChatIntegration
{
	public class WeChatPayRefundResult : Result
	{
		public decimal RefundAmount { get; internal set; }
		public string? OriginalResult { get; internal set; }
	}
}
