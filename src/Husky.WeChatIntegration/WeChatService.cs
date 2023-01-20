using System.Net.Http;
using Husky.WeChatIntegration.ServiceCategorized;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Husky.WeChatIntegration
{
	public class WeChatService
	{
		public WeChatService(WeChatOptions options, IHttpContextAccessor httpContextAccessor, IMemoryCache cache) {
			_http = httpContextAccessor;
			_cache = cache;
			_options = options;
		}

		private readonly IHttpContextAccessor _http;
		private readonly IMemoryCache _cache;
		private readonly WeChatOptions _options;

		public WeChatOptions Options => _options;
		public WeChatJsApiService JsApi() => new(_options, _http, _cache);
		public WeChatUserService User() => new(_options);
		public WeChatAuthService Auth() => new(_options);
		public WeChatPayService Pay() => new(_options);


		// HttpClient

		private static HttpClient? _httpClient;
		public static HttpClient HttpClient => _httpClient ??= new HttpClient();

		// CertifiedHttpClient

		private static HttpClient? _certifiedHttpClient;
		public static HttpClient CertifiedHttpClient(WeChatOptions options) => _certifiedHttpClient ??= new HttpClient(
			string.IsNullOrEmpty(options.MerchantCertFile)
				? new CertifiedWxpayHttpClientHandler(options.MerchantId!)
				: new CertifiedWxpayHttpClientHandler(options.MerchantId, options.MerchantCertFile)
		);
	}
}
