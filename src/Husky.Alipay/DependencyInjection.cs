using System;
using Alipay.AopSdk.AspnetCore;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddAlipay(this HuskyDI husky, AlipayOptions options) {
			husky.Services.AddAlipay(x => x.SetOption(options));
			return husky;
		}

		public static HuskyDI AddAlipay(this HuskyDI husky, Action<AlipayOptions> options) {
			husky.Services.AddAlipay(options);
			return husky;
		}
	}
}