namespace Husky.WeChatIntegration.Models.Pay
{
	public class WeChatPayOrderModelUnifiedResult : Result
	{
		public string? PrepayId { get; internal set; }
		public string? CodeUrl { get; internal set; }
		public string? OriginalResult { get; internal set; }
	}
}
