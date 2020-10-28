using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Husky.WeChatIntegration.ServiceCategorized
{
	public class WeChatJsApiService
	{
		public WeChatJsApiService(WeChatAppConfig wechatConfig, IHttpContextAccessor httpContextAccessor, IMemoryCache cache) {
			_wechatConfig = wechatConfig;
			_httpContextAccessor = httpContextAccessor;
			_cache = cache;
		}

		private readonly WeChatAppConfig _wechatConfig;
		private readonly IMemoryCache _cache;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private static readonly HttpClient _httpClient = new HttpClient();

		public async Task<WeChatGeneralAccessToken> GetMobilePlatformGeneralAccessTokenAsync() {
			_wechatConfig.RequireMobilePlatformSettings();

			return await _cache.GetOrCreate(_wechatConfig.MobilePlatformAppId + nameof(GetMobilePlatformGeneralAccessTokenAsync), async entry => {
				var url = $"https://api.weixin.qq.com/cgi-bin/token" +
					  $"?grant_type=client_credential" +
					  $"&appid={_wechatConfig.MobilePlatformAppId}" +
					  $"&secret={_wechatConfig.MobilePlatformAppSecret}";

				try {
					var json = await _httpClient.GetStringAsync(url);
					var d = JsonConvert.DeserializeObject<dynamic>(json);

					var ok = d.errcode == null || (int)d.errcode == 0;
					entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(ok ? (int)d.expires_in : 1));

					return new WeChatGeneralAccessToken {
						Ok = ok,
						Message = d.errmsg,

						AccessToken = d.access_token,
						ExpiresIn = d.expires_in
					};
				}
				catch ( Exception e ) {
					return new WeChatGeneralAccessToken {
						Ok = false,
						Message = e.Message
					};
				}
			});
		}

		public async Task<string> GetJsApiTicketAsync() {
			_wechatConfig.RequireMobilePlatformSettings();

			return await _cache.GetOrCreate(_wechatConfig.MobilePlatformAppId + nameof(GetJsApiTicketAsync), async entry => {
				var accessToken = await GetMobilePlatformGeneralAccessTokenAsync();
				if ( !accessToken.Ok ) {
					return accessToken.Message ?? "无法获得 access_token";
				}

				var url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket" + $"?access_token={accessToken.AccessToken}&type=jsapi";
				try {
					var json = await _httpClient.GetStringAsync(url);
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
			_wechatConfig.RequireMobilePlatformSettings();

			var config = new WeChatJsApiConfig {
				AppId = _wechatConfig.MobilePlatformAppId!,
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
			_wechatConfig.RequireMobilePlatformSettings();

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
