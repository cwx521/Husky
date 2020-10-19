using System;
using Alipay.AopSdk.AspnetCore;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyInjector AddAlipay(this HuskyInjector husky, AlipayOptions options) {
			husky.Services.AddAlipay(x => x.SetOption(options));
			return husky;
		}

		public static HuskyInjector AddAlipay(this HuskyInjector husky, Action<AlipayOptions> setupAction) {
			husky.Services.AddAlipay(setupAction);
			return husky;
		}
	}
}