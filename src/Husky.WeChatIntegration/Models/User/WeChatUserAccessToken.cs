namespace Husky.WeChatIntegration
{
	public record WeChatUserAccessToken
	{
		public string OpenId { get; internal init; } = null!;
		public string? UnionId { get; internal set; }
		public string AccessToken { get; internal init; } = null!;
		public string? RefreshToken { get; internal init; }
	}
}
