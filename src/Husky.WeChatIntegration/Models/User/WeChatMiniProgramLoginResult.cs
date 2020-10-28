namespace Husky.WeChatIntegration
{
	public class WeChatMiniProgramLoginResult : Result
	{
		public string OpenId { get; set; } = null!;
		public string SessionKey { get; set; } = null!;
		public string? UnionId { get; set; }
	}
}
