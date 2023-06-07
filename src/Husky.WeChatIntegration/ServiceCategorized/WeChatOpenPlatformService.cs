using System;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Husky.WeChatIntegration.ServiceCategorized
{
	public class WeChatOpenPlatformService
	{
		public string CreateWebQrCodeLoginScript(WeChatAppIdSecret appIdSecret, string redirectUrl, string cssUrl) {
			appIdSecret.NotNull();
			appIdSecret.MustBe(WeChatAppRegion.OpenPlatform);

			var elementId = "_" + Crypto.RandomString();
			var html = @"<div id='" + elementId + @"'></div>
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
								id: '" + elementId + @"',
								appid: '" + appIdSecret.AppId + @"',
								redirect_uri: '" + redirectUrl + @"',
								state: '" + Crypto.Encrypt(DateTime.Now.ToString("yyyy-M-d H:mm:ss"), iv: appIdSecret.AppId!) + @"',
								href: '" + cssUrl + @"',
								style: ''
							});
						}
					})();
				</script>";
			return html;
		}

		public static async Task<Result<WeChatUserAccessToken>> GetUserAccessTokenAsync(WeChatAppIdSecret appIdSecret, string code) {
			appIdSecret.NotNull();
			appIdSecret.MustBe(WeChatAppRegion.OpenPlatform);

			var url = $"https://api.weixin.qq.com/sns/oauth2/access_token" +
					  $"?appid={appIdSecret.AppId}" +
					  $"&secret={appIdSecret.AppSecret}" +
					  $"&code={code}" +
					  $"&grant_type=authorization_code";

			try {
				var json = await WeChatService.HttpClient.GetStringAsync(url);
				var d = JsonConvert.DeserializeObject<dynamic>(json)!;

				if (d.errcode != null && (int)d.errcode != 0) {
					return new Failure<WeChatUserAccessToken>((int)d.errcode + ": " + d.errmsg);
				}
				return new Success<WeChatUserAccessToken> {
					Data = new WeChatUserAccessToken {
						AccessToken = d.access_token,
						RefreshToken = d.refresh_token,
						OpenId = d.openid
					}
				};
			}
			catch (Exception e) {
				return new Failure<WeChatUserAccessToken>(e.Message);
			}
		}
	}
}
