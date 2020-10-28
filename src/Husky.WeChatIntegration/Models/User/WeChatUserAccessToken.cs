namespace Husky.WeChatIntegration
{
	public class WeChatUserAccessToken : Result
	{
		public string AccessToken { get; set; } = null!;
		public string? RefreshToken { get; set; }
		public string OpenId { get; set; } = null!;
	}
}
