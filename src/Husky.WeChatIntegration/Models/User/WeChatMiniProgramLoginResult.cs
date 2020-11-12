namespace Husky.WeChatIntegration
{
	public record WeChatMiniProgramLoginResult
	{
		public string OpenId { get; internal init; } = null!;
		public string SessionKey { get; internal init; } = null!;
		public string? UnionId { get; internal init; }
	}
}
