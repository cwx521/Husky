namespace Husky.Alipay
{
	public record AlipayOrderPrecreationResult
	{
		public string DesktopPagePaymentUrl { get; internal init; } = null!;
		public string MobileWebPaymentUrl { get; internal init; } = null!;
	}
}
