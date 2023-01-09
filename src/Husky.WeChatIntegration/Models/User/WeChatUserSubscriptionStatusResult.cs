using System;

namespace Husky.WeChatIntegration
{
	public record WeChatUserSubscriptionStatusResult
	{
		public bool Subscribed { get; internal init; }
		public DateTime? SubscribeTime { get; internal init; }
		public string? SubscribeScene { get; internal init; }
		public string OpenId { get; internal init; } = null!;
		public string? UnionId { get; internal init; }
	}
}
