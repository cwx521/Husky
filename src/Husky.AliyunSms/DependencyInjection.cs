using Husky.AliyunSms;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyInjector AddAliyunSms(this HuskyInjector husky, AliyunSmsSettings settings) {
			husky.Services.AddSingleton(new AliyunSmsSender(settings));
			return husky;
		}
	}
}
