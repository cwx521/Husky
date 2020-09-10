using System;

namespace Husky.WeChatIntegration
{
	public class WeChatPayOrderModel
	{
		public string AppId { get; set; } = null!;
		public string OpenId { get; set; } = null!;
		public string OrderId { get; set; } = null!;
		public string NotifyUrl { get; set; } = null!;
		public string Body { get; set; } = null!;
		public string IPAddress { get; set; } = null!;
		public string? Attach { get; set; }
		public decimal Amount { get; set; }
		public bool AllowCreditCard { get; set; } = true;
		public TimeSpan Expiration { get; set; } = TimeSpan.FromHours(2);
		public string Device { get; set; } = "WEB";
		public WeChatPayTradeType TradeType { get; set; } = WeChatPayTradeType.JSAPI;
	}
}
