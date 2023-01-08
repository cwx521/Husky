using System;

namespace Husky.WeChatIntegration
{
	public class WeChatPayOrderMicroPayModel
	{
		public string AppId { get; set; } = null!;
		public string AuthCode { get; set; } = null!;
		public string OrderNo { get; set; } = null!;
		public decimal Amount { get; set; }
		public string Body { get; set; } = null!;
		public string IPAddress { get; set; } = null!;
		public string? Attach { get; set; }
		public bool AllowCreditCard { get; set; } = true;
		public TimeSpan Expiration { get; set; } = TimeSpan.FromHours(2);
	}
}
