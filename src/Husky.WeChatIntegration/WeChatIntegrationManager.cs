using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Husky.WeChatIntegration
{
	public class WeChatIntegrationManager
	{
		public WeChatIntegrationManager(WeChatAppSettings settings, IHttpContextAccessor http, IMemoryCache cache) {
			Settings = settings;
			_http = http.HttpContext;
			_cache = cache;
		}

		private readonly HttpContext _http;
		private readonly IMemoryCache _cache;

		public WeChatAppSettings Settings { get; private set; }

		public string CreateLoginQrCode(string redirectUri, string styleSheetUrl, WeChatAppSettings overrideSettings = null) {
			var settings = overrideSettings ?? Settings;
			var targetElementId = "_" + Crypto.RandomString();
			var html = @"<div id='" + targetElementId + @"'></div>
				<script type='text/javascript' src='https://res.wx.qq.com/connect/zh_CN/htmledition/js/wxLogin.js'></script>
				<script type='text/javascript'>
					(function loadWxLogin() {
						if (typeof WxLogin !== 'function') {			
							setTimeout(loadWxLogin, 50);	
						}
						else {
							var obj = new WxLogin({
								self_redirect: false,
								scope: 'snsapi_login',
								id: '" + targetElementId + @"',
								appid: '" + Settings.AppId + @"',
								redirect_uri: '" + redirectUri + @"',
								state: '" + Crypto.Encrypt(DateTime.Now.ToString("yyyy-M-d H:mm:ss")) + @"',
								href: '" + styleSheetUrl + @"',
								style: ''
							});
						}		
					})();
				</script>";
			return html;
		}

		public string CreateMpAutoLoginSteppingUrl(string redirectUrl, string scope = "snsapi_userinfo", WeChatAppSettings overrideSettings = null) {
			var settings = overrideSettings ?? Settings;
			return $"https://open.weixin.qq.com/connect/oauth2/authorize" +
				   $"?appid={settings.AppId}" +
				   $"&redirect_uri={HttpUtility.UrlEncode(redirectUrl)}" +
				   $"&response_type=code" +
				   $"&scope={scope}" +
				   $"&state={Crypto.Encrypt(DateTime.Now.ToString("yyyy-M-d H:mm:ss"))}" +
				   $"#wechat_redirect";
		}

		public WeChatUserAccessToken GetUserAccessToken(string code, WeChatAppSettings overrideSettings = null) {
			var settings = overrideSettings ?? Settings;
			var url = $"https://api.weixin.qq.com/sns/oauth2/access_token" +
					  $"?appid={settings.AppId}&secret={settings.AppSecret}&code={code}&grant_type=authorization_code";
			return GetUserAccessTokenFromResolvedUrl(url);
		}
		public WeChatUserAccessToken RefreshUserAccessToken(string refreshToken, WeChatAppSettings overrideSettings = null) {
			var settings = overrideSettings ?? Settings;
			var url = $"https://api.weixin.qq.com/sns/oauth2/refresh_token" +
					  $"?appid={settings.AppId}&refresh_token={refreshToken}&grant_type=refresh_token";
			return GetUserAccessTokenFromResolvedUrl(url);
		}
		private WeChatUserAccessToken GetUserAccessTokenFromResolvedUrl(string url) {
			using ( var client = new WebClient() ) {
				var json = client.DownloadString(url);
				var d = JsonConvert.DeserializeObject<dynamic>(json);

				if ( d.access_token == null ) {
					return null;
				}
				return new WeChatUserAccessToken {
					AccessToken = d.access_token,
					RefreshToken = d.refresh_token,
					OpenId = d.openid
				};
			}
		}

		public WeChatUserInfo GetUserInfo(string code, WeChatAppSettings overrideSettings = null) {
			return GetUserInfo(GetUserAccessToken(code, overrideSettings));
		}
		public WeChatUserInfo GetUserInfo(WeChatUserAccessToken token) {
			return GetUserInfo(token.OpenId, token.AccessToken);
		}
		public WeChatUserInfo GetUserInfo(string openId, string accessToken) {
			var url = $"https://api.weixin.qq.com/sns/userinfo" + $"?access_token={accessToken}&openid={openId}&lang=zh-CN";
			using ( var client = new WebClient() ) {
				var json = client.DownloadString(url);
				var d = JsonConvert.DeserializeObject<dynamic>(json);
				if ( d.errcode != null && d.errcode != 0 ) {
					return null;
				}
				return new WeChatUserInfo {
					OpenId = d.openid,
					UnionId = d.unionid,
					NickName = d.nickname,
					Sex = d.sex == 2 ? Sex.Female : Sex.Male,
					Province = d.province,
					City = d.city,
					Country = d.country,
					HeadImageUrl = ((string)d.headimgurl)?.Replace("http://", "https://")
				};
			}
		}

		public WeChatGeneralAccessToken GetGeneralAccessToken(WeChatAppSettings overrideSettings = null) {
			var settings = overrideSettings ?? Settings;
			return _cache.GetOrCreate(settings.AppId + nameof(GetGeneralAccessToken), entry => {

				var url = $"https://api.weixin.qq.com/cgi-bin/token" +
					  $"?grant_type=client_credential" +
					  $"&appid={settings.AppId}" +
					  $"&secret={settings.AppSecret}";

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

		public string GetJsapiTicket(WeChatAppSettings overrideSettings = null) {
			var settings = overrideSettings ?? Settings;
			return _cache.GetOrCreate(settings.AppId + nameof(GetJsapiTicket), entry => {

				var accessToken = GetGeneralAccessToken(overrideSettings);
				var url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket" + $"?access_token={accessToken.AccessToken}&type=jsapi";

				using ( var client = new WebClient() ) {
					var json = client.DownloadString(url);
					var d = JsonConvert.DeserializeObject<dynamic>(json);
					entry.SetAbsoluteExpiration(TimeSpan.FromSeconds((int)d.expires_in));
					return d.ticket;
				}
			});
		}

		public WeChatJsapiConfig BuildWeChatJsapiConfig(WeChatAppSettings overrideSettings = null) {
			var settings = overrideSettings ?? Settings;
			var config = new WeChatJsapiConfig {
				AppId = settings.AppId,
				NonceStr = Crypto.RandomString(16),
				Timestamp = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).Ticks / 1000,
				Ticket = GetJsapiTicket(overrideSettings),
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

		public string CreateWeChatJsapiConfigScript(params string[] jsApiList) {
			var cfg = BuildWeChatJsapiConfig();
			if ( jsApiList == null || jsApiList.Length == 0 ) {
				jsApiList = new[] {
					"updateAppMessageShareData",
					"updateTimelineShareData",
					"onMenuShareAppMessage",
					"onMenuShareTimeline",
					"openLocation",
					"getLocation",
					"scanQRCode",
					"chooseWXPay",
					"getNetworkType",
					"chooseImage",
					"hideMenuItems",
					"closeWindow"
				};
			}
			return @"<script type='text/javascript' src='https://res2.wx.qq.com/open/js/jweixin-1.4.0.js'></script>
				<script type='text/javascript'>
					function loadWeChatConfig() {
						if (typeof(wx) == undefined) {
							setTimeout(loadWeChatConfig, 50);
						}
						else {
							wx.config({
								debug: false,
								appId: '" + cfg.AppId + @"',
								timestamp: " + cfg.Timestamp + @",
								nonceStr: '" + cfg.NonceStr + @"',
								signature: '" + cfg.Signature + @"',
								jsApiList: [" + string.Join(',', jsApiList.Select(x => $"'{x}'")) + @"]
							});
						}
					}
					setTimeout(loadWeChatConfig, 50);
				</script>";
		}

		public WeChatJsapiPayParameter BuildWeChatJsapiPayParameter(string prepayId, WeChatAppSettings overrideSettings = null) {
			var settings = overrideSettings ?? Settings;
			var nonceStr = Crypto.RandomString(32);
			var timeStamp = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).Ticks / 1000;

			var sb = new StringBuilder();
			sb.Append("appId=" + settings.AppId);
			sb.Append("&nonceStr=" + nonceStr);
			sb.Append("&package=prepay_id=" + prepayId);
			sb.Append("&signType=MD5");
			sb.Append("&timeStamp=" + timeStamp);
			sb.Append("&key=" + settings.MerchantSecret);
			var paySign = Crypto.MD5(sb.ToString()).ToUpper();

			return new WeChatJsapiPayParameter {
				timestamp = timeStamp,
				nonceStr = nonceStr,
				package = $"prepay_id={prepayId}",
				signType = "MD5",
				paySign = paySign
			};
		}

		public string GetApiResultXml(string wechatApiUrl, Dictionary<string, string> parameters, string overrideMerchantSecret = null) {
			var sb = new StringBuilder();

			var orderedNames = parameters.Keys.OrderBy(x => x).ToArray();
			foreach ( var name in orderedNames ) {
				sb.Append(name + "=" + parameters[name] + "&");
			}
			sb.Append("key=" + overrideMerchantSecret ?? Settings.MerchantSecret);

			var tobeSigned = sb.ToString();
			parameters.Add("sign", Crypto.MD5(tobeSigned).ToUpper());

			sb.Clear();
			sb.Append("<xml>");
			foreach ( var item in parameters ) {
				sb.AppendFormat("<{0}>{1}</{0}>", item.Key, item.Value);
			}
			sb.Append("</xml>");
			var xml = sb.ToString();

			using var webClient = new WebClient();
			return webClient.UploadString(wechatApiUrl, xml);
		}
	}
}
