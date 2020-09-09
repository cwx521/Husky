namespace Husky.Alipay
{
	public class AlipayOrderQueryResult
	{
		public string AlipayTradeNo { get; internal set; } = null!;
		public string AlipayBuyerLogonId { get; internal set; } = null!;
		public decimal TotalAmount { get; internal set; }
	}
}
