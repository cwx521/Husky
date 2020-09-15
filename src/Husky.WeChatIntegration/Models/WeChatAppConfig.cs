using System;

namespace Husky.WeChatIntegration
{
	public class WeChatAppConfig
	{
		public string? OpenPlatformAppId { get; set; }
		public string? OpenPlatformAppSecret { get; set; }
		public string? MobilePlatformAppId { get; set; }
		public string? MobilePlatformAppSecret { get; set; }
		public string? MiniProgramAppId { get; set; }
		public string? MiniProgramAppSecret { get; set; }
		public string? MerchantId { get; set; }
		public string? MerchantSecret { get; set; }

		internal void RequireOpenPlatformSettings() {
			_ = OpenPlatformAppId ?? throw new ArgumentException($"{nameof(OpenPlatformAppId)} 没有配置");
			_ = OpenPlatformAppSecret ?? throw new ArgumentException($"{nameof(OpenPlatformAppSecret)} 没有配置");
		}
		internal void RequireMobilePlatformSettings() {
			_ = MobilePlatformAppId ?? throw new ArgumentException($"{nameof(MobilePlatformAppId)} 没有配置");
			_ = MobilePlatformAppSecret ?? throw new ArgumentException($"{nameof(MobilePlatformAppSecret)} 没有配置");
		}
		internal void RequireMiniProgramSettings() {
			_ = MiniProgramAppId ?? throw new ArgumentException($"{nameof(MiniProgramAppId)} 没有配置");
			_ = MiniProgramAppSecret ?? throw new ArgumentException($"{nameof(MiniProgramAppSecret)} 没有配置");
		}
		internal void RequireMerchantSettings() {
			_ = MerchantId ?? throw new ArgumentException($"{nameof(MerchantId)} 没有配置");
			_ = MerchantSecret ?? throw new ArgumentException($"{nameof(MerchantSecret)} 没有配置");
		}
	}
}
