using System;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Html;
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
					HeadImageUrl = d.headimgurl
				};
			}
		}

		public WeChatGeneralAccessToken GetGeneralAccessToken(WeChatAppSettings overrideSettings = null) {
			var settings = overrideSettings ?? Settings;
			return _cache.GetOrCreate(settings.AppId + nameof(GetGeneralAccessToken), entry => {
				entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(7150));
				var url = $"https://api.weixin.qq.com/cgi-bin/token" +
					  $"?grant_type=client_credential" +
					  $"&appid={settings.AppId}" +
					  $"&secret={settings.AppSecret}";
				using ( var client = new WebClient() ) {
					var json = client.DownloadString(url);
					var d = JsonConvert.DeserializeObject<dynamic>(json);
					return new WeChatGeneralAccessToken {
						AccessToken = d.access_token
					};
				}
			});
		}

		public string GetJsapiTicket(WeChatAppSettings overrideSettings = null) {
			var settings = overrideSettings ?? Settings;
			return _cache.GetOrCreate(settings.AppId + nameof(GetJsapiTicket), entry => {
				entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(7150));
				var accessToken = GetGeneralAccessToken(overrideSettings);
				var url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket" + $"?access_token={accessToken.AccessToken}&type=jsapi";
				using ( var client = new WebClient() ) {
					var json = client.DownloadString(url);
					var d = JsonConvert.DeserializeObject<dynamic>(json);
					return d.ticket;
				}
			});
		}

		public WeChatJsapiConfig BuildWeChatJsapiConfig(WeChatAppSettings overrideSettings = null) {
			var settings = overrideSettings ?? Settings;
			var config = new WeChatJsapiConfig {
				AppId = settings.AppId,
				Ticket = GetJsapiTicket(overrideSettings),
				NonceStr = Crypto.RandomString(16),
				Timestamp = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).Ticks / 1000
			};

			var sb = new StringBuilder();
			sb.Append("jsapi_ticket=" + config.Ticket);
			sb.Append("&noncestr=" + config.NonceStr);
			sb.Append("&timestamp=" + config.Timestamp.ToString());
			sb.Append("&url=" + _http.Request.UrlBase() + _http.Request.Url());

			config.RawString = sb.ToString();
			config.Signature = Crypto.SHA1(config.RawString);
			return config;
		}
	}
}
