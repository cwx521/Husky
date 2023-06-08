namespace Husky.WeChatIntegration
{
	public record WeChatUserPhoneResult
	{
		public string PhoneNumber { get; internal init; } = null!;
		public string PurePhoneNumber { get; internal init; } = null!;
		public string CountryCode { get; internal init; } = null!;
	}
}
