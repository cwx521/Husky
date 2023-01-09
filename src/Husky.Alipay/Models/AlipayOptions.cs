namespace Husky.Alipay
{
	public class AlipayOptions
	{
		public string AppId { get; set; } = null!;
		public string PrivateKey { get; set; } = null!;
		public string AlipayPublicKey { get; set; } = null!;
		public string? DefaultNotifyUrl { get; set; }
		public string GatewayUrl { get; set; } = "https://openapi.alipay.com/gateway.do";
		public string SignType { get; set; } = "RSA2";
		public string CharSet { get; set; } = "UTF-8";
		public string Format { get; set; } = "json";
		public string Version { get; set; } = "1.0";
	}
}
