namespace Husky.WeChatIntegration
{
	public record WeChatUserInfoResult
	{
		public string OpenId { get; internal init; } = null!;
		public string? UnionId { get; internal init; }
		public string NickName { get; internal init; } = null!;
		public Sex Sex { get; internal init; }
		public string? Province { get; internal init; }
		public string? City { get; internal init; }
		public string? Country { get; internal init; }
		public string HeadImageUrl { get; internal init; } = null!;
	}
}
