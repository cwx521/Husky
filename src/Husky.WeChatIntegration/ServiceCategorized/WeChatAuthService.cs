using System;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace Husky.WeChatIntegration.ServiceCategorized
{
	public class WeChatAuthService
	{
		public WeChatAuthService(WeChatOptions options) {
			_options = options;
		}

		private readonly WeChatOptions _options;

		public string CreateWebQrCodeLoginScript(string redirectUrl, string cssUrl) {
			_options.RequireOpenPlatformSettings();

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
								appid: '" + _options.OpenPlatformAppId + @"',
								redirect_uri: '" + redirectUrl + @"',
								state: '" + Crypto.Encrypt(DateTime.Now.ToString("yyyy-M-d H:mm:ss"), iv: _options.OpenPlatformAppId!) + @"',
								href: '" + cssUrl + @"',
								style: ''
							});
						}
					})();
				</script>";
			return html;
		}

		public string CreateMobilePlatformAutoLoginUrl(string redirectUrl, string scope = "snsapi_userinfo") {
			_options.RequireMobilePlatformSettings();

			return $"https://open.weixin.qq.com/connect/oauth2/authorize" +
				   $"?appid={_options.MobilePlatformAppId}" +
				   $"&redirect_uri={HttpUtility.UrlEncode(redirectUrl)}" +
				   $"&response_type=code" +
				   $"&scope={scope}" +
				   $"&state={Crypto.Encrypt(DateTime.Now.ToString("yyyy-M-d H:mm:ss"), iv: _options.MobilePlatformAppId!)}" +
				   $"#wechat_redirect";
		}

		public async Task<Result<WeChatMiniProgramLoginResult>> ProceedMiniProgramLoginAsync(string code) {
			_options.RequireMiniProgramSettings();

			var url = $"https://api.weixin.qq.com/sns/jscode2session" +
					  $"?appid={_options.MiniProgramAppId}" +
					  $"&secret={_options.MiniProgramAppSecret}" +
					  $"&js_code={code}" +
					  $"&grant_type=authorization_code";

			try {
				var json = await DefaultHttpClient.Instance.GetStringAsync(url);
				var d = JsonConvert.DeserializeObject<dynamic>(json)!;

				if (d.errcode != null && (int)d.errcode != 0) {
					return new Failure<WeChatMiniProgramLoginResult>((int)d.errcode + ": " + d.errmsg);
				}
				return new Success<WeChatMiniProgramLoginResult> {
					Data = new WeChatMiniProgramLoginResult {
						OpenId = d.openid,
						UnionId = d.unionid,
						SessionKey = d.session_key
					}
				};
			}
			catch (Exception e) {
				return new Failure<WeChatMiniProgramLoginResult>(e.Message);
			}
		}
	}
}
