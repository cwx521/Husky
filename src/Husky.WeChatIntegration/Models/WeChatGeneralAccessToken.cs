namespace Husky.WeChatIntegration
{
	public record WeChatGeneralAccessToken
	{
		public string AccessToken { get; internal init; } = null!;
		public int ExpiresIn { get; internal init; }
	}
}
