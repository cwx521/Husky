namespace Husky.WeChatIntegration
{
	public class WeChatJsApiConfig
	{
		public string AppId { get; internal set; } = null!;
		public string Ticket { get; internal set; } = null!;
		public string NonceStr { get; internal set; } = null!;
		public long Timestamp { get; internal set; }
		public string Signature { get; internal set; } = null!;
		public string RawString { get; internal set; } = null!;
	}
}
