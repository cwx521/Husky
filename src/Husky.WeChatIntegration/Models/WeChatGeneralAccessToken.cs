using System;

namespace Husky.WeChatIntegration
{
	public record WeChatGeneralAccessToken
	{
		public string AccessToken { get; internal init; } = null!;
		public DateTime Expires { get; internal init; }
	}
}
