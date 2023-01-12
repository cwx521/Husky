namespace Husky.Alipay
{
	public class AlipayTradeMicroPayModel
	{
		public decimal Amount { get; set; }
		public string TradeNo { get; set; } = null!;
		public string Subject { get; set; } = null!;
		public bool AllowCreditCard { get; set; } = true;

		public string AuthCode { get; set; } = null!;
		public string Scene { get; set; } = "bar_code";
	}
}
