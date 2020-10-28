namespace Husky.WeChatIntegration
{
	public class WeChatUserResult : Result
	{
		public string OpenId { get; set; } = null!;
		public string? UnionId { get; set; }
		public string NickName { get; set; } = null!;
		public Sex Sex { get; set; }
		public string? Province { get; set; }
		public string? City { get; set; }
		public string? Country { get; set; }
		public string HeadImageUrl { get; set; } = null!;
	}
}
