namespace Husky.WeChatIntegration
{
	public class WeChatPayQueryOrderResult
	{
		public string AppId { get; set; } = null!;
		public string MerchantId { get; set; } = null!;
		public string TransactionId { get; set; } = null!;
		public string OpenId { get; set; } = null!;
		public decimal Amount { get; set; }
	}
}
