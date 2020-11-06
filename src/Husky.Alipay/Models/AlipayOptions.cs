namespace Husky.Alipay
{
	public class AlipayOptions
	{
		public string PartnerId { get; set; } = null!;
		public string AppId { get; set; } = null!;
		public string PrivateKey { get; set; } = null!;
		public string AlipayPublicKey { get; set; } = null!;
		public string GatewayUrl { get; set; } = "https://openapi.alipay.com/gateway.do";
		public string? DefaultNotifyUrl { get; set; }
		public string SignType { get; set; } = "RSA2";
		public string CharSet { get; set; } = "UTF-8";
		public string Format => "json";
		public string Version => "1.0";
	}
}
