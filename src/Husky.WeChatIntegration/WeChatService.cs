using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Husky.WeChatIntegration.ServiceCategorized;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

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
		public WeChatMobilePlatformService MobilePlatform() => new(_options, _http, _cache);
		public WeChatMiniProgramService MiniProgram() => new();
		public WeChatOpenPlatformService OpenPlatform() => new();
		public WeChatPayService Pay() => new(_options.WxPay!);


		// HttpClient

		private static HttpClient? _httpClient;
		public static HttpClient HttpClient => _httpClient ??= new HttpClient();

		// CertifiedHttpClient

		private static HttpClient? _certifiedHttpClient;
		public static HttpClient CertifiedHttpClient(WxPayOptions wxPayOptions) => _certifiedHttpClient ??= new HttpClient(
			string.IsNullOrEmpty(wxPayOptions.MerchantCertFile)
				? new CertifiedWxpayHttpClientHandler(wxPayOptions.MerchantId!)
				: new CertifiedWxpayHttpClientHandler(wxPayOptions.MerchantId, wxPayOptions.MerchantCertFile)
		);


		// Get Access Token

		public async Task<Result<WeChatGeneralAccessToken>> GetAccessTokenAsync(WeChatAppIdSecret appIdSecret) {
			appIdSecret.NotNull();
			appIdSecret.MustHaveSecret();

			return await _cache.GetOrCreate<Task<Result<WeChatGeneralAccessToken>>>(appIdSecret.AppId + nameof(GetAccessTokenAsync), async entry => {
				var url = _options.GeneralAccessTokenManagedCentral ?? "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential";
				url += url.Contains('?') ? '&' : '?';
				url += $"appid={appIdSecret.AppId}&secret={appIdSecret.AppSecret}";

				try {
					var json = await WeChatService.HttpClient.GetStringAsync(url);
					var d = JsonConvert.DeserializeObject<dynamic>(json)!;

					var ok = d.errcode == null || (int)d.errcode == 0;
					if (!ok) {
						entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(1));
						return new Failure<WeChatGeneralAccessToken>((int)d.errcode + ": " + d.errmsg);
					}

					var timeSpan = d.expires_at != null
						? ((DateTime)d.expires_at).Subtract(DateTime.Now)
						: TimeSpan.FromSeconds((int)d.expires_in);

					entry.SetAbsoluteExpiration(timeSpan);

					return new Success<WeChatGeneralAccessToken> {
						Data = new() {
							AccessToken = d.access_token,
							Expires = DateTime.Now.Add(timeSpan)
						}
					};
				}
				catch (Exception e) {
					return new Failure<WeChatGeneralAccessToken>(e.Message);
				}
			})!;
		}

		public async Task<Result<WeChatGeneralAccessToken>> GetStableAccessTokenAsync(WeChatAppIdSecret appIdSecret) {
			appIdSecret.NotNull();
			appIdSecret.MustHaveSecret();

			return await _cache.GetOrCreate<Task<Result<WeChatGeneralAccessToken>>>(appIdSecret.AppId + nameof(GetStableAccessTokenAsync), async entry => {
				var url = "https://api.weixin.qq.com/cgi-bin/stable_token";
				var parameters = new {
					grant_type = "client_credential",
					appid = appIdSecret.AppId,
					secret = appIdSecret.AppSecret
				};

				try {
					var response = await WeChatService.HttpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(parameters)));
					var json = await response.Content.ReadAsStringAsync();
					var d = JsonConvert.DeserializeObject<dynamic>(json)!;

					var ok = d.errcode == null || (int)d.errcode == 0;
					if (!ok) {
						entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(1));
						return new Failure<WeChatGeneralAccessToken>((int)d.errcode + ": " + d.errmsg);
					}

					var timeSpan = TimeSpan.FromSeconds((int)d.expires_in);
					entry.SetAbsoluteExpiration(timeSpan);

					return new Success<WeChatGeneralAccessToken> {
						Data = new() {
							AccessToken = d.access_token,
							Expires = DateTime.Now.Add(timeSpan)
						}
					};
				}
				catch (Exception e) {
					return new Failure<WeChatGeneralAccessToken>(e.Message);
				}
			})!;
		}
	}
}
