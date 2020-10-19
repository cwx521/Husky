#pragma warning disable IDE1006 // Naming Styles

namespace Husky.WeChatIntegration
{

	public class WeChatPayJsApiParameter
	{
		public string id { get; set; } = null!;
		public string nonceStr { get; set; } = null!;
		public string package { get; set; } = null!;
		public string signType { get; set; } = null!;
		public long timestamp { get; set; }
		public string paySign { get; set; } = null!;
	}
}
