namespace Husky.Alipay
{
	public class AlipayOrderModel
	{
		public decimal Amount { get; set; }
		public string OrderNo { get; set; } = null!;
		public string Subject { get; set; } = null!;
		public string? Body { get; set; }

		public string CallbackUrl { get; set; } = null!;
		public string NotifyUrl { get; set; } = null!;
	}
}
