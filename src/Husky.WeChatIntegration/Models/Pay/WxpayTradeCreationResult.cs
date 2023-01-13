namespace Husky.WeChatIntegration.Models.Pay
{
	public record WxpayTradeCreationResult
	{
		public string? PrepayId { get; internal init; }
		public string? CodeUrl { get; internal init; }
		public string? OriginalResult { get; internal init; }
	}
}
