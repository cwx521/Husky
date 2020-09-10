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
	public class WeChatService
	{
		public WeChatService(WeChatAppConfig wechatConfig, IHttpContextAccessor http, IMemoryCache cache) {
			_wechatConfig = wechatConfig;
			_http = http.HttpContext;
			_cache = cache;
		}

		private readonly WeChatAppConfig _wechatConfig;
		private readonly HttpContext _http;
		private readonly IMemoryCache _cache;

		public string CreateLoginQrCode(string redirectUri, string styleSheetUrl) {
			_wechatConfig.RequireOpenPlatformSettings();

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
								appid: '" + _wechatConfig.OpenPlatformAppId + @"',
								redirect_uri: '" + redirectUri + @"',
								state: '" + Crypto.Encrypt(DateTime.Now.ToString("yyyy-M-d H:mm:ss"), iv: _wechatConfig.OpenPlatformAppId!) + @"',
								href: '" + styleSheetUrl + @"',
								style: ''
							});
						}		
					})();
				</script>";
			return html;
		}

		public string CreateMobilePlatformAutoLoginSteppingUrl(string redirectUrl, string scope = "snsapi_userinfo") {
			_wechatConfig.RequireMobilePlatformSettings();

			return $"https://open.weixin.qq.com/connect/oauth2/authorize" +
				   $"?appid={_wechatConfig.MobilePlatformAppId}" +
				   $"&redirect_uri={HttpUtility.UrlEncode(redirectUrl)}" +
				   $"&response_type=code" +
				   $"&scope={scope}" +
				   $"&state={Crypto.Encrypt(DateTime.Now.ToString("yyyy-M-d H:mm:ss"), iv: _wechatConfig.MobilePlatformAppId!)}" +
				   $"#wechat_redirect";
		}

		public WeChatUserAccessToken? GetOpenPlatformUserAccessToken(string code) {
			_wechatConfig.RequireOpenPlatformSettings();

			return GetUserAccessToken(code, new WeChatAppIdSecret {
				AppId = _wechatConfig.OpenPlatformAppId,
				AppSecret = _wechatConfig.OpenPlatformAppSecret
			});
		}
		public WeChatUserAccessToken? GetMobilePlatformUserAccessToken(string code) {
			_wechatConfig.RequireMobilePlatformSettings();

			return GetUserAccessToken(code, new WeChatAppIdSecret {
				AppId = _wechatConfig.MobilePlatformAppId,
				AppSecret = _wechatConfig.MobilePlatformAppSecret
			});
		}
		public WeChatUserAccessToken? GetUserAccessToken(string code, WeChatAppIdSecret idSecret) {
			idSecret.CheckNull();

			var url = $"https://api.weixin.qq.com/sns/oauth2/access_token" +
					  $"?appid={idSecret.AppId}&secret={idSecret.AppSecret}&code={code}&grant_type=authorization_code";
			return GetUserAccessTokenFromResolvedUrl(url);
		}

		public WeChatUserAccessToken? RefreshOpenPlatformUserAccessToken(string refreshToken) {
			_wechatConfig.RequireOpenPlatformSettings();

			return RefreshUserAccessToken(refreshToken, new WeChatAppIdSecret {
				AppId = _wechatConfig.OpenPlatformAppId,
				AppSecret = _wechatConfig.OpenPlatformAppSecret
			});
		}
		public WeChatUserAccessToken? RefreshMobilePlatformUserAccessToken(string refreshToken) {
			_wechatConfig.RequireMobilePlatformSettings();

			return RefreshUserAccessToken(refreshToken, new WeChatAppIdSecret {
				AppId = _wechatConfig.MobilePlatformAppId,
				AppSecret = _wechatConfig.MobilePlatformAppSecret
			});
		}
		public WeChatUserAccessToken? RefreshUserAccessToken(string refreshToken, WeChatAppIdSecret idSecret) {
			idSecret.CheckNull();

			var url = $"https://api.weixin.qq.com/sns/oauth2/refresh_token" +
					  $"?appid={idSecret.AppId}&refresh_token={refreshToken}&grant_type=refresh_token";
			return GetUserAccessTokenFromResolvedUrl(url);
		}

		private WeChatUserAccessToken? GetUserAccessTokenFromResolvedUrl(string url) {
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

		public WeChatUserInfo? GetUserInfo(WeChatUserAccessToken token) {
			return GetUserInfo(token.OpenId, token.AccessToken);
		}
		public WeChatUserInfo? GetUserInfo(string openId, string accessToken) {
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
					HeadImageUrl = ((string)d.headimgurl).Replace("http://", "https://")
				};
			}
		}

		public WeChatMiniProgramLoginResult MiniProgramLogin(string code) {
			_wechatConfig.RequireMiniProgramSettings();

			var url = $"https://api.weixin.qq.com/sns/jscode2session" +
					  $"?appid={_wechatConfig.MiniProgramAppId}" +
					  $"&secret={_wechatConfig.MiniProgramAppSecret}" +
					  $"&js_code={code}" +
					  $"&grant_type=authorization_code";

			using ( var client = new WebClient() ) {
				var json = client.DownloadString(url);
				var d = JsonConvert.DeserializeObject<dynamic>(json);
				return new WeChatMiniProgramLoginResult {
					OpenId = d.openid,
					UnionId = d.unionid,
					SessionKey = d.session_key
				};
			}
		}

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

		public string CreateJsApiConfigScript(params string[] enableJsApiNames) {
			_wechatConfig.RequireMobilePlatformSettings();

			var cfg = CreateJsApiConfig();
			if ( enableJsApiNames == null || enableJsApiNames.Length == 0 ) {
				enableJsApiNames = new[] {
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
					"previewImage",
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
								jsApiList: [" + string.Join(',', enableJsApiNames.Select(x => $"'{x}'")) + @"]
							});
						}
					}
					setTimeout(loadWeChatConfig, 50);
				</script>";
		}
	}
}
