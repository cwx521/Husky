using System;

namespace Husky.WeChatIntegration
{
	public record WeChatAppIdSecret
	{
		public string AppId { get; init; } = null!;
		public string? AppSecret { get; init; }
		public WeChatAppRegion? Region { get; init; }
		public string? Alias { get; set; }

		internal void NotNull() {
			_ = AppId ?? throw new ArgumentNullException(nameof(AppId));
		}

		internal void MustHaveSecret() {
			_ = AppSecret ?? throw new ArgumentNullException(nameof(AppSecret));
		}

		internal void MustBe(WeChatAppRegion region) {
			if (Region != region) {
				throw new ArgumentException($"The {nameof(Region)} must be {region} here.");
			}
		}
	}
}
