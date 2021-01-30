using System;

namespace Husky.WeChatIntegration
{
	public class WeChatOptions
	{
		public string? OpenPlatformAppId { get; set; }
		public string? OpenPlatformAppSecret { get; set; }
		public string? MobilePlatformAppId { get; set; }
		public string? MobilePlatformAppSecret { get; set; }
		public string? MiniProgramAppId { get; set; }
		public string? MiniProgramAppSecret { get; set; }
		public string? MerchantId { get; set; }
		public string? MerchantSecret { get; set; }
		public string? GeneralAccessTokenManagedCentral { get; set; }

		internal void RequireOpenPlatformSettings() {
			_ = OpenPlatformAppId ?? throw new ArgumentException($"缺少 {nameof(OpenPlatformAppId)} 配置");
			_ = OpenPlatformAppSecret ?? throw new ArgumentException($"缺少 {nameof(OpenPlatformAppSecret)} 配置");
		}
		internal void RequireMobilePlatformSettings() {
			_ = MobilePlatformAppId ?? throw new ArgumentException($"缺少 {nameof(MobilePlatformAppId)} 配置");
			_ = MobilePlatformAppSecret ?? throw new ArgumentException($"缺少 {nameof(MobilePlatformAppSecret)} 配置");
		}
		internal void RequireMiniProgramSettings() {
			_ = MiniProgramAppId ?? throw new ArgumentException($"缺少 {nameof(MiniProgramAppId)} 配置");
			_ = MiniProgramAppSecret ?? throw new ArgumentException($"缺少 {nameof(MiniProgramAppSecret)} 配置");
		}
		internal void RequireMerchantSettings() {
			_ = MerchantId ?? throw new ArgumentException($"缺少 {nameof(MerchantId)} 配置");
			_ = MerchantSecret ?? throw new ArgumentException($"缺少 {nameof(MerchantSecret)} 配置");
		}
	}
}
