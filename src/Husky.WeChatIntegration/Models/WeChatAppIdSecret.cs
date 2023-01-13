using System;

namespace Husky.WeChatIntegration
{
	public record WeChatAppIdSecret
	{
		public string? AppId { get; init; }
		public string? AppSecret { get; init; }
		public WeChatRegion? Region { get; init; }

		internal void NotNull() {
			_ = AppId ?? throw new ArgumentNullException(nameof(AppId));
			_ = AppSecret ?? throw new ArgumentNullException(nameof(AppSecret));
		}
	}
}
