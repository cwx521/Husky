using System;

namespace Husky.WeChatIntegration
{
	public class WeChatPayModel
	{
		public string OpenId { get; set; } = null!;
		public string OrderId { get; set; } = null!;
		public string NotifyUrl { get; set; } = null!;
		public string? Body { get; set; }
		public decimal Amount { get; set; }
		public string? Attach { get; set; }
		public TimeSpan Expiration { get; set; } = TimeSpan.FromHours(2);
	}
}
