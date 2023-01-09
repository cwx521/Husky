namespace Husky.Alipay
{
	public class AlipayOrderF2FPayModel
	{
		public decimal Amount { get; set; }
		public string AuthCode { get; set; } = null!;
		public string OrderNo { get; set; } = null!;
		public string Subject { get; set; } = null!;
		public string? Body { get; set; }
		public bool AllowCreditCard { get; set; } = true;
		public string Scene { get; set; } = "bar_code";
	}
}
