namespace Husky.Alipay
{
	public class AlipayNotifyResult : Result
	{
		public string? OrderNo { get; internal set; }
		public string? AlipayTradeNo { get; internal set; }
		public string? AlipayBuyerId { get; internal set; }
		public decimal Amount { get; internal set; }
	}
}
