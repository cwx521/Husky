using System;
using Husky.Alipay;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyInjector AddAlipay(this HuskyInjector husky, AlipayOptions options) {
			husky.Services.AddSingleton(new AlipayService(options));
			return husky;
		}

		public static HuskyInjector AddAlipay(this HuskyInjector husky, Action<AlipayOptions> setupAction) {
			var options = new AlipayOptions();
			setupAction(options);
			husky.Services.AddSingleton(new AlipayService(options));
			return husky;
		}
	}
}