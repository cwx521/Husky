namespace Husky.Alipay
{
	public class AlipayTradeCreationModel
	{
		public decimal Amount { get; set; }
		public string TradeNo { get; set; } = null!;
		public string Subject { get; set; } = null!;
		public bool AllowCreditCard { get; set; } = true;

		public string? NotifyUrl { get; set; }
		public string? ReturnUrl { get; set; }
	}
}
