namespace Husky.WeChatIntegration
{
	public class WeChatJsapiConfig
	{
		public string AppId { get; internal set; }
		public string Ticket { get; internal set; }
		public string NonceStr { get; internal set; }
		public long Timestamp { get; internal set; }
		public string Signature { get; internal set; }
		public string RawString { get; internal set; }
	}
}
