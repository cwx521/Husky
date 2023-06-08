using System.Collections.Generic;

namespace Husky.WeChatIntegration
{
	public class WeChatOptions
	{
		public List<WeChatAppIdSecret> AppIdSecrets { get; set; } = new List<WeChatAppIdSecret>();
		public WxPayOptions? WxPay { get; set; }
		public string? GeneralAccessTokenManagedCentral { get; set; }
	}
}
