using Husky.WeChatIntegration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddWeChatIntegration(this HuskyDI husky, WeChatAppSettings settings) {
			husky.Services.AddScoped(svc =>
				new WeChatIntegrationManager(
					settings,
					svc.GetRequiredService<IHttpContextAccessor>(),
					svc.GetRequiredService<IMemoryCache>()
				)
			);
			return husky;
		}
	}
}
