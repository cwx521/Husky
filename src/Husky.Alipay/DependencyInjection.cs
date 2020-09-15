using Alipay.AopSdk.AspnetCore;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddAlipay(this HuskyDI husky, AlipayOptions alipayOptions) {
			husky.Services.AddAlipay(x => x.SetOption(alipayOptions));
			return husky;
		}
	}
}