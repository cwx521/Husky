using System;
using Husky.Alipay;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyServiceHub AddAlipay(this HuskyServiceHub husky, AlipayOptions options) {
			husky.Services.AddSingleton(new AlipayService(options));
			return husky;
		}

		public static HuskyServiceHub AddAlipay(this HuskyServiceHub husky, Action<AlipayOptions> setupAction) {
			var options = new AlipayOptions();
			setupAction(options);
			husky.Services.AddSingleton(new AlipayService(options));
			return husky;
		}
	}
}