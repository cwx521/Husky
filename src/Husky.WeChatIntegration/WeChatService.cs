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
			WechatAppConfig = wechatConfig;
		}

		private readonly IHttpContextAccessor _http;
		private readonly IMemoryCache _cache;

		public WeChatAppConfig WechatAppConfig { get; }

		public WeChatUserService UserService() => new WeChatUserService(WechatAppConfig);
		public WeChatLoginService LoginService() => new WeChatLoginService(WechatAppConfig);
		public WeChatJsApiService JsApiService() => new WeChatJsApiService(WechatAppConfig, _http, _cache);
		public WeChatPayService PayService() => new WeChatPayService(WechatAppConfig);
	}
}
