using Husky.WeChatIntegration.ServiceCategorized;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Husky.WeChatIntegration
{
	public class WeChatService
	{
		public WeChatService(WeChatAppConfig wechatConfig, IHttpContextAccessor httpContextAccessor, IMemoryCache cache) {
			_http = httpContextAccessor;
			_cache = cache;
			Config = wechatConfig;
		}

		private readonly IHttpContextAccessor _http;
		private readonly IMemoryCache _cache;

		public WeChatAppConfig Config { get; }

		public WeChatJsApiService JsApi() => new WeChatJsApiService(Config, _http, _cache);
		public WeChatUserService User() => new WeChatUserService(Config);
		public WeChatAuthService Auth() => new WeChatAuthService(Config);
		public WeChatPayService Pay() => new WeChatPayService(Config);
	}
}
