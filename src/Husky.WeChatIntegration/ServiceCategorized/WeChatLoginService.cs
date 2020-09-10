using System;
using System.Net;
using System.Web;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Husky.WeChatIntegration.ServiceCategorized
{
	public class WeChatLoginService
	{
		public WeChatLoginService(WeChatAppConfig wechatConfig, IHttpContextAccessor http) {
			_http = http.HttpContext;
			_wechatConfig = wechatConfig;
		}

		private readonly HttpContext _http;
		private readonly WeChatAppConfig _wechatConfig;

		public string CreateWebQrCodeLoginScript(string redirectUri, string styleSheetUrl) {
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

		public string CreateMobilePlatformAutoLoginUrl(string redirectUrl, string scope = "snsapi_userinfo") {
			_wechatConfig.RequireMobilePlatformSettings();

			return $"https://open.weixin.qq.com/connect/oauth2/authorize" +
				   $"?appid={_wechatConfig.MobilePlatformAppId}" +
				   $"&redirect_uri={HttpUtility.UrlEncode(redirectUrl)}" +
				   $"&response_type=code" +
				   $"&scope={scope}" +
				   $"&state={Crypto.Encrypt(DateTime.Now.ToString("yyyy-M-d H:mm:ss"), iv: _wechatConfig.MobilePlatformAppId!)}" +
				   $"#wechat_redirect";
		}

		public WeChatMiniProgramLoginResult ProceedMiniProgramLogin(string code) {
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
	}
}
