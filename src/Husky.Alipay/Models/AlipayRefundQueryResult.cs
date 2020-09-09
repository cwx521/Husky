namespace Husky.Alipay
{
	public class AlipayRefundQueryResult
	{
		public string AlipayTradeNo { get; internal set; } = null!;
		public decimal RefundAmount { get; internal set; }
		public decimal TotalAmount { get; internal set; }
	}
}
