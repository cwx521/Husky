using System;

namespace Husky.WeChatIntegration
{
	public class WeChatAppIdSecret
	{
		public string? AppId { get; set; } 
		public string? AppSecret { get; set; }


		internal void CheckNull() {
			_ = AppId ?? throw new ArgumentNullException(nameof(AppId));
			_ = AppSecret ?? throw new ArgumentNullException(nameof(AppSecret));
		}
	}
}
