namespace Husky.WeChatIntegration
{
	public class WeChatGeneralAccessToken : Result
	{
		public string AccessToken { get; set; } = null!;
		public int ExpiresIn { get; set; }
	}
}
