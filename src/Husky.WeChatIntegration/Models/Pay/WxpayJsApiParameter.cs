#pragma warning disable IDE1006 // Naming Styles

namespace Husky.WeChatIntegration
{
	public class WxpayJsApiParameter
	{
		public string id { get; set; } = null!;
		public string nonceStr { get; internal set; } = null!;
		public string package { get; internal set; } = null!;
		public string signType { get; internal set; } = null!;
		public long timestamp { get; internal set; }
		public string paySign { get; internal set; } = null!;
	}
}
