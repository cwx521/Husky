using Husky.AliyunSms;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.DependencyInjection
{
	public static class DependencyInjection
	{
		public static HuskyDependencyInjectionHub AddAliyunSms(this HuskyDependencyInjectionHub husky, AliyunSmsSettings settings) {
			husky.Services.AddSingleton(new AliyunSmsSender(settings));
			return husky;
		}
	}
}
