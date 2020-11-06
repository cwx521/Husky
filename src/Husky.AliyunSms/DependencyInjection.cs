using System;
using Husky.Sms;
using Husky.Sms.AliyunSms;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyInjector AddAliyunSms(this HuskyInjector husky, AliyunSmsOptions options) {
			husky.Services.AddSingleton<ISmsSender>(new AliyunSmsSender(options));
			return husky;
		}

		public static HuskyInjector AddAliyunSms(this HuskyInjector husky, Action<AliyunSmsOptions> setupAction) {
			var options = new AliyunSmsOptions();
			setupAction(options);
			husky.Services.AddSingleton<ISmsSender>(new AliyunSmsSender(options));
			return husky;
		}
	}
}
