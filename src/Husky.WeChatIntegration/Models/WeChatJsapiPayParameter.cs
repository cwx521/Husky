namespace Husky.WeChatIntegration
{
	public class WeChatJsapiPayParameter
	{
		public string id { get; set; }
		public string nonceStr { get; set; }
		public string package { get; set; }
		public string signType { get; set; }
		public long timestamp { get; set; }
		public string paySign { get; set; }
	}
}
