using System;
using System.Net;
using System.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Husky.WeChatIntegration
{
	public class WeChatIntegrationManager
	{
		public WeChatIntegrationManager(WeChatOpenPlatformSettings settings, IHttpContextAccessor http) {
			_http = http.HttpContext;
			Settings = settings;
		}

		private readonly HttpContext _http;
		public WeChatOpenPlatformSettings Settings { get; private set; }

		public string CreateLoginQrCode(string redirectUri, string styleSheetUrl, WeChatOpenPlatformSettings overrideSettings = null) {
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

		public string CreateMpAutoLoginSteppingUrl(string redirectUrl, string scope = "snsapi_userinfo", WeChatOpenPlatformSettings overrideSettings = null) {
			var settings = overrideSettings ?? Settings;
			return $"https://open.weixin.qq.com/connect/oauth2/authorize" +
				   $"?appid={settings.AppId}" +
				   $"&redirect_uri={HttpUtility.UrlEncode(redirectUrl)}" +
				   $"&response_type=code" +
				   $"&scope={scope}" +
				   $"&state={Crypto.Encrypt(DateTime.Now.ToString("yyyy-M-d H:mm:ss"))}" +
				   $"#wechat_redirect";
		}

		public WeChatAccessToken GetAccessToken(string code, WeChatOpenPlatformSettings overrideSettings = null) {
			var settings = overrideSettings ?? Settings;
			var url = $"https://api.weixin.qq.com/sns/oauth2/access_token" +
					  $"?appid={settings.AppId}&secret={settings.AppSecret}&code={code}&grant_type=authorization_code";
			return GetAccessTokenFromResolvedUrl(url);
		}
		public WeChatAccessToken RefreshAccessToken(string refreshToken, WeChatOpenPlatformSettings overrideSettings = null) {
			var settings = overrideSettings ?? Settings;
			var url = $"https://api.weixin.qq.com/sns/oauth2/refresh_token" +
					  $"?appid={settings.AppId}&refresh_token={refreshToken}&grant_type=refresh_token";
			return GetAccessTokenFromResolvedUrl(url);
		}
		private WeChatAccessToken GetAccessTokenFromResolvedUrl(string url) {
			using ( var client = new WebClient() ) {
				var json = client.DownloadString(url);
				var d = JsonConvert.DeserializeObject<dynamic>(json);

				if ( d.access_token == null ) {
					return null;
				}
				return new WeChatAccessToken {
					AccessToken = d.access_token,
					RefreshToken = d.refresh_token,
					OpenId = d.openid
				};
			}
		}

		public WeChatUserInfo GetUserInfo(WeChatAccessToken token) {
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
					Sex = d.sex == 2 ? Sex.Male : Sex.Female,
					Province = d.province,
					City = d.city,
					Country = d.country,
					HeadImageUrl = d.headimgurl
				};
			}
		}
	}
}
