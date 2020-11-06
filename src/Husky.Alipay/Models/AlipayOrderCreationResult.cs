namespace Husky.Alipay
{
	public class AlipayOrderCreationResult
	{
		public string DesktopPagePaymentUrl { get; internal set; } = null!;
		public string MobileWebPaymentUrl { get; internal set; } = null!;
	}
}
