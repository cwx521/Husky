using Husky.WeChatIntegration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.DependencyInjection
{
	public static class DependencyInjection
	{
		public static HuskyDependencyInjectionHub AddWeChatIntegration(this HuskyDependencyInjectionHub husky, WeChatOpenPlatformSettings settings) {
			husky.Services.AddSingleton(svc => new WeChatIntegrationManager(settings, svc.GetRequiredService<IHttpContextAccessor>()));
			return husky;
		}
	}
}
