using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Husky.WeChatIntegration.ServiceCategorized
{
	public class WeChatJsApiService
	{
		public WeChatJsApiService(WeChatOptions options, IHttpContextAccessor httpContextAccessor, IMemoryCache cache) {
			_options = options;
			_httpContextAccessor = httpContextAccessor;
			_cache = cache;
		}

		private readonly WeChatOptions _options;
		private readonly IMemoryCache _cache;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public async Task<Result<WeChatGeneralAccessToken>> GetMobilePlatformGeneralAccessTokenAsync() {
			_options.RequireMobilePlatformSettings();

			return await _cache.GetOrCreate<Task<Result<WeChatGeneralAccessToken>>>(_options.MobilePlatformAppId + nameof(GetMobilePlatformGeneralAccessTokenAsync), async entry => {
				var url = $"https://api.weixin.qq.com/cgi-bin/token" +
					  $"?grant_type=client_credential" +
					  $"&appid={_options.MobilePlatformAppId}" +
					  $"&secret={_options.MobilePlatformAppSecret}";

				try {
					var json = await DefaultHttpClient.Instance.GetStringAsync(url);
					var d = JsonConvert.DeserializeObject<dynamic>(json);

					var ok = d.errcode == null || (int)d.errcode == 0;
					entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(ok ? (int)d.expires_in : 1));

					if ( !ok ) {
						return new Failure<WeChatGeneralAccessToken>((string)d.errmsg);
					}
					return new Success<WeChatGeneralAccessToken> {
						Data = new WeChatGeneralAccessToken {
							AccessToken = d.access_token,
							ExpiresIn = d.expires_in
						}
					};
				}
				catch ( Exception e ) {
					return new Failure<WeChatGeneralAccessToken>(e.Message);
				}
			});
		}

		public async Task<string> GetJsApiTicketAsync() {
			_options.RequireMobilePlatformSettings();

			return await _cache.GetOrCreate(_options.MobilePlatformAppId + nameof(GetJsApiTicketAsync), async entry => {
				var result = await GetMobilePlatformGeneralAccessTokenAsync();
				if ( !result.Ok ) {
					return result.Message ?? "无法获得 access_token";
				}

				var url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket" + $"?access_token={result.Data?.AccessToken}&type=jsapi";
				try {
					var json = await DefaultHttpClient.Instance.GetStringAsync(url);
					var d = JsonConvert.DeserializeObject<dynamic>(json);

					var ok = d.errcode == null || (int)d.errcode == 0;
					entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(ok ? (int)d.expires_in : 1));

					return d.ticket ?? d.errmsg;
				}
				catch ( Exception e ) {
					return e.Message;
				}
			});
		}

		public async Task<WeChatJsApiConfig> CreateJsApiConfigAsync() {
			if ( _httpContextAccessor.HttpContext == null ) {
				throw new InvalidProgramException("Can not call this method when IHttpContextAccessor.HttpContext is null.");
			}

			_options.RequireMobilePlatformSettings();

			var config = new WeChatJsApiConfig {
				AppId = _options.MobilePlatformAppId!,
				NonceStr = Crypto.RandomString(16),
				Timestamp = DateTime.Now.Timestamp(),
				Ticket = await GetJsApiTicketAsync(),
			};

			var sb = new StringBuilder();
			sb.Append("jsapi_ticket=" + config.Ticket);
			sb.Append("&noncestr=" + config.NonceStr);
			sb.Append("&timestamp=" + config.Timestamp.ToString());
			sb.Append("&url=" + _httpContextAccessor.HttpContext.Request.FullUrl().Split('#').First());

			config.RawString = sb.ToString();
			config.Signature = Crypto.SHA1(config.RawString);
			return config;
		}

		private const string _defaultEnabledJsApiNames = "updateAppMessageShareData,updateTimelineShareData,onMenuShareAppMessage,onMenuShareTimeline,openLocation,getLocation,scanQRCode,chooseWXPay,getNetworkType,chooseImage,previewImage,hideMenuItems,closWindow";
		public async Task<string> CreateJsApiScriptAsync(string enableJsApiNames = _defaultEnabledJsApiNames) {
			_options.RequireMobilePlatformSettings();

			var cfg = await CreateJsApiConfigAsync();
			return @"<script type='text/javascript' src='https://res2.wx.qq.com/open/js/jweixin-1.4.0.js'></script>
				<script type='text/javascript'>
					function configWeChatJsApi() {
						if (typeof(wx) == undefined) {
							setTimeout(configWeChatJsApi, 50);
						}
						else {
							wx.config({
								debug: false,
								appId: '" + cfg.AppId + @"',
								timestamp: " + cfg.Timestamp + @",
								nonceStr: '" + cfg.NonceStr + @"',
								signature: '" + cfg.Signature + @"',
								jsApiList: [" + string.Join(',', enableJsApiNames.Split(',', '|').Select(x => $"'{x.Trim()}'")) + @"]
							});
						}
					}
					setTimeout(configWeChatJsApi, 50);
				</script>";
		}
	}
}
