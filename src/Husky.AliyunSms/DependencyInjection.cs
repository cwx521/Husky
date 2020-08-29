using Husky.AliyunSms;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddAliyunSms(this HuskyDI husky, AliyunSmsSettings settings) {
			husky.Services.AddSingleton(new AliyunSmsSender(settings));
			return husky;
		}
	}
}
