using Husky.WeChatIntegration.ServiceCategorized;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Husky.WeChatIntegration
{
	public class WeChatService
	{
		public WeChatService(WeChatOptions wechatOptions, IHttpContextAccessor httpContextAccessor, IMemoryCache cache) {
			_http = httpContextAccessor;
			_cache = cache;
			Options = wechatOptions;
		}

		private readonly IHttpContextAccessor _http;
		private readonly IMemoryCache _cache;

		public WeChatOptions Options { get; }

		public WeChatJsApiService JsApi() => new WeChatJsApiService(Options, _http, _cache);
		public WeChatUserService User() => new WeChatUserService(Options);
		public WeChatAuthService Auth() => new WeChatAuthService(Options);
		public WeChatPayService Pay() => new WeChatPayService(Options);
	}
}
