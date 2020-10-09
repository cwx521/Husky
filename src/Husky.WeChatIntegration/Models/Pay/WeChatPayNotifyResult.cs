namespace Husky.WeChatIntegration
{
	public class WeChatPayNotifyResult : Result
	{
		public string OrderNo { get; internal set; } = null!;
		public string TransactionId { get; internal set; } = null!;
		public string OpenId { get; internal set; } = null!;
		public decimal Amount { get; internal set; }
		public string? Attach { get; internal set; }
		public string? OriginalResult { get; internal set; }
	}
}
