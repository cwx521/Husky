﻿using Husky.WeChatIntegration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyInjector AddWeChatIntegration(this HuskyInjector husky, WeChatOptions options) {
			husky.Services.AddScoped(svc =>
				new WeChatService(
					options,
					svc.GetRequiredService<IHttpContextAccessor>(),
					svc.GetRequiredService<IMemoryCache>()
				)
			);
			return husky;
		}
	}
}
