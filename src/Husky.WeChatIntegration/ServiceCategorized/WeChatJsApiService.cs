using System;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Husky.WeChatIntegration.ServiceCategorized
{
	public class WeChatJsApiService
	{
		public WeChatJsApiService(WeChatAppConfig wechatConfig, IHttpContextAccessor http, IMemoryCache cache) {
			_wechatConfig = wechatConfig;
			_http = http.HttpContext;
			_cache = cache;
		}

		private readonly WeChatAppConfig _wechatConfig;
		private readonly HttpContext _http;
		private readonly IMemoryCache _cache;

		public WeChatGeneralAccessToken GetMobilePlatformGeneralAccessToken() {
			_wechatConfig.RequireMobilePlatformSettings();

			return _cache.GetOrCreate(_wechatConfig.MobilePlatformAppId + nameof(GetMobilePlatformGeneralAccessToken), entry => {
				var url = $"https://api.weixin.qq.com/cgi-bin/token" +
					  $"?grant_type=client_credential" +
					  $"&appid={_wechatConfig.MobilePlatformAppId}" +
					  $"&secret={_wechatConfig.MobilePlatformAppSecret}";

				using ( var client = new WebClient() ) {
					var json = client.DownloadString(url);
					var d = JsonConvert.DeserializeObject<dynamic>(json);

					entry.SetAbsoluteExpiration(TimeSpan.FromSeconds((int)d.expires_in));

					return new WeChatGeneralAccessToken {
						AccessToken = d.access_token,
						ExpiresIn = d.expires_in
					};
				}
			});
		}

		public string GetJsApiTicket() {
			_wechatConfig.RequireMobilePlatformSettings();

			return _cache.GetOrCreate(_wechatConfig.MobilePlatformAppId + nameof(GetJsApiTicket), entry => {
				var accessToken = GetMobilePlatformGeneralAccessToken();
				var url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket" + $"?access_token={accessToken.AccessToken}&type=jsapi";

				using ( var client = new WebClient() ) {
					var json = client.DownloadString(url);
					var d = JsonConvert.DeserializeObject<dynamic>(json);

					entry.SetAbsoluteExpiration(TimeSpan.FromSeconds((int)d.expires_in));
					return d.ticket;
				}
			});
		}

		public WeChatJsApiConfig CreateJsApiConfig() {
			_wechatConfig.RequireMobilePlatformSettings();

			var config = new WeChatJsApiConfig {
				AppId = _wechatConfig.MobilePlatformAppId!,
				NonceStr = Crypto.RandomString(16),
				Timestamp = DateTime.Now.Timestamp(),
				Ticket = GetJsApiTicket(),
			};

			var sb = new StringBuilder();
			sb.Append("jsapi_ticket=" + config.Ticket);
			sb.Append("&noncestr=" + config.NonceStr);
			sb.Append("&timestamp=" + config.Timestamp.ToString());
			sb.Append("&url=" + _http.Request.FullUrl().Split('#').First());

			config.RawString = sb.ToString();
			config.Signature = Crypto.SHA1(config.RawString);
			return config;
		}

		public string CreateJsApiScript(string enableJsApiNames = "updateAppMessageShareData,updateTimelineShareData,onMenuShareAppMessage,onMenuShareTimeline,openLocation,getLocation,scanQRCode,chooseWXPay,getNetworkType,chooseImage,previewImage,hideMenuItems,closWindow") {
			_wechatConfig.RequireMobilePlatformSettings();

			var cfg = CreateJsApiConfig();
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
