using Husky.WeChatIntegration.ServiceCategorized;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Husky.WeChatIntegration
{
	public class WeChatService
	{
		public WeChatService(WeChatAppConfig wechatConfig, IHttpContextAccessor http, IMemoryCache cache) {
			_http = http;
			_cache = cache;
			Config = wechatConfig;
		}

		private readonly IHttpContextAccessor _http;
		private readonly IMemoryCache _cache;

		public WeChatAppConfig Config { get; }

		public WeChatUserService UserService() => new WeChatUserService(Config);
		public WeChatLoginService LoginService() => new WeChatLoginService(Config);
		public WeChatJsApiService JsApiService() => new WeChatJsApiService(Config, _http, _cache);
		public WeChatPayService PayService() => new WeChatPayService(Config);
	}
}
