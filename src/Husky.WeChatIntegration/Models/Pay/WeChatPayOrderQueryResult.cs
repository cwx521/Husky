namespace Husky.WeChatIntegration
{
	public class WeChatPayOrderQueryResult : Result
	{
		public string? OpenId { get; internal set; }
		public decimal Amount { get; internal set; }
		public bool HasRefund { get; internal set; }
		public string? TransactionId { get; internal set; }
		public string? OriginalResult { get; internal set; }
	}
}
