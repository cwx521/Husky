using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Husky.WeChatIntegration
{
	public class WeChatIntegrationManager
	{
		public WeChatIntegrationManager(WeChatOpenPlatformSettings settings, IHttpContextAccessor http) {
			_settings = settings;
			_http = http.HttpContext;
		}

		private WeChatOpenPlatformSettings _settings;
		private HttpContext _http;

		public HtmlString RenderLoginQrCode(string redirectUri, string styleSheetUrl) {
			var targetElementId = "_" + Crypto.RandomString();
			var html = @"<div id='" + targetElementId + @"'></div>
				<script type='text/javascript' src='https://res.wx.qq.com/connect/zh_CN/htmledition/js/wxLogin.js'></script>
				<script type='text/javascript'>
					var obj = new WxLogin({
						self_redirect: true,
						scope: 'snsapi_login',
						id: '" + targetElementId + @"',
						appid: '" + _settings.AppId + @"',
						redirect_uri: '" + redirectUri + @"',
						state: '" + Crypto.Encrypt(DateTime.Now.ToString("yyyy-M-d H:mm:ss")) + @"',
						href: '" + styleSheetUrl + @"',
						style: ''
					});
				</script>";
			return new HtmlString(html);
		}

		public WeChatAccessToken GetAccessToken(string code) {
			var url = $"https://api.weixin.qq.com/sns/oauth2/access_token" +
					  $"?appid={_settings.AppId}&secret={_settings.AppSecret}&code={code}&grant_type=authorization_code";
			return GetAccessTokenFromResolvedUrl(url);
		}
		public WeChatAccessToken RefreshAccessToken(string refreshToken) {
			var url = $"https://api.weixin.qq.com/sns/oauth2/refresh_token" +
					  $"?appid={_settings.AppId}&refresh_token={refreshToken}&grant_type=refresh_token";
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
			var url = $"https://api.weixin.qq.com/sns/userinfo" + $"?access_token={accessToken}&openid={openId}";
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
