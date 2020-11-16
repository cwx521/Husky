namespace Husky.Alipay
{
	public class AlipayOrderModel
	{
		public decimal Amount { get; set; }
		public string OrderNo { get; set; } = null!;
		public string Subject { get; set; } = null!;
		public string? Body { get; set; }
		public bool AllowCreditCard { get; set; } = true;

		public string? NotifyUrl { get; set; }
		public string? ReturnUrl { get; set; }
	}
}
