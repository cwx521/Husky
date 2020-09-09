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
		public WeChatIntegrationManager(WeChatAppSettings wechatSettings, IHttpContextAccessor http, IMemoryCache cache) {
			_wechatSettings = wechatSettings;
			_http = http.HttpContext;
			_cache = cache;
		}

		private readonly WeChatAppSettings _wechatSettings;
		private readonly HttpContext _http;
		private readonly IMemoryCache _cache;

		public string CreateLoginQrCode(string redirectUri, string styleSheetUrl) {
			_wechatSettings.RequireOpenPlatformSettings();

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
								appid: '" + _wechatSettings.OpenPlatformAppId + @"',
								redirect_uri: '" + redirectUri + @"',
								state: '" + Crypto.Encrypt(DateTime.Now.ToString("yyyy-M-d H:mm:ss"), iv: _wechatSettings.OpenPlatformAppId!) + @"',
								href: '" + styleSheetUrl + @"',
								style: ''
							});
						}		
					})();
				</script>";
			return html;
		}

		public string CreateMobilePlatformAutoLoginSteppingUrl(string redirectUrl, string scope = "snsapi_userinfo") {
			_wechatSettings.RequireMobilePlatformSettings();

			return $"https://open.weixin.qq.com/connect/oauth2/authorize" +
				   $"?appid={_wechatSettings.MobilePlatformAppId}" +
				   $"&redirect_uri={HttpUtility.UrlEncode(redirectUrl)}" +
				   $"&response_type=code" +
				   $"&scope={scope}" +
				   $"&state={Crypto.Encrypt(DateTime.Now.ToString("yyyy-M-d H:mm:ss"), iv: _wechatSettings.MobilePlatformAppId!)}" +
				   $"#wechat_redirect";
		}

		public WeChatUserAccessToken? GetOpenPlatformUserAccessToken(string code) {
			_wechatSettings.RequireOpenPlatformSettings();

			return GetUserAccessToken(code, new WeChatAppIdSecret {
				AppId = _wechatSettings.OpenPlatformAppId,
				AppSecret = _wechatSettings.OpenPlatformAppSecret
			});
		}
		public WeChatUserAccessToken? GetMobilePlatformUserAccessToken(string code) {
			_wechatSettings.RequireMobilePlatformSettings();

			return GetUserAccessToken(code, new WeChatAppIdSecret {
				AppId = _wechatSettings.MobilePlatformAppId,
				AppSecret = _wechatSettings.MobilePlatformAppSecret
			});
		}
		public WeChatUserAccessToken? GetUserAccessToken(string code, WeChatAppIdSecret idSecret) {
			idSecret.CheckNull();

			var url = $"https://api.weixin.qq.com/sns/oauth2/access_token" +
					  $"?appid={idSecret.AppId}&secret={idSecret.AppSecret}&code={code}&grant_type=authorization_code";
			return GetUserAccessTokenFromResolvedUrl(url);
		}

		public WeChatUserAccessToken? RefreshOpenPlatformUserAccessToken(string refreshToken) {
			_wechatSettings.RequireOpenPlatformSettings();

			return RefreshUserAccessToken(refreshToken, new WeChatAppIdSecret {
				AppId = _wechatSettings.OpenPlatformAppId,
				AppSecret = _wechatSettings.OpenPlatformAppSecret
			});
		}
		public WeChatUserAccessToken? RefreshMobilePlatformUserAccessToken(string refreshToken) {
			_wechatSettings.RequireMobilePlatformSettings();

			return RefreshUserAccessToken(refreshToken, new WeChatAppIdSecret {
				AppId = _wechatSettings.MobilePlatformAppId,
				AppSecret = _wechatSettings.MobilePlatformAppSecret
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
			_wechatSettings.RequireMiniProgramSettings();

			var url = $"https://api.weixin.qq.com/sns/jscode2session" +
					  $"?appid={_wechatSettings.MiniProgramAppId}" +
					  $"&secret={_wechatSettings.MiniProgramAppSecret}" +
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
			_wechatSettings.RequireMobilePlatformSettings();

			return _cache.GetOrCreate(_wechatSettings.MobilePlatformAppId + nameof(GetMobilePlatformGeneralAccessToken), entry => {
				var url = $"https://api.weixin.qq.com/cgi-bin/token" +
					  $"?grant_type=client_credential" +
					  $"&appid={_wechatSettings.MobilePlatformAppId}" +
					  $"&secret={_wechatSettings.MobilePlatformAppSecret}";

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

		public string GetJsapiTicket() {
			_wechatSettings.RequireMobilePlatformSettings();

			return _cache.GetOrCreate(_wechatSettings.MobilePlatformAppId + nameof(GetJsapiTicket), entry => {
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

		public WeChatJsapiConfig CreateJsapiConfig() {
			_wechatSettings.RequireMobilePlatformSettings();

			var config = new WeChatJsapiConfig {
				AppId = _wechatSettings.MobilePlatformAppId!,
				NonceStr = Crypto.RandomString(16),
				Timestamp = DateTime.Now.Timestamp(),
				Ticket = GetJsapiTicket(),
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

		public string CreateJsapiConfigScript(params string[] enableJsapiNames) {
			_wechatSettings.RequireMobilePlatformSettings();

			var cfg = CreateJsapiConfig();
			if ( enableJsapiNames == null || enableJsapiNames.Length == 0 ) {
				enableJsapiNames = new[] {
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
								jsApiList: [" + string.Join(',', enableJsapiNames.Select(x => $"'{x}'")) + @"]
							});
						}
					}
					setTimeout(loadWeChatConfig, 50);
				</script>";
		}

		public WeChatJsapiPayParameter JsapiPayCreateParameter(string prepayId) {
			_wechatSettings.RequireMobilePlatformSettings();
			_wechatSettings.RequireMerchantSettings();

			var nonceStr = Crypto.RandomString(32);
			var timeStamp = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).Ticks / 1000;

			var sb = new StringBuilder();
			sb.Append("appId=" + _wechatSettings.MobilePlatformAppId);
			sb.Append("&nonceStr=" + nonceStr);
			sb.Append("&package=prepay_id=" + prepayId);
			sb.Append("&signType=MD5");
			sb.Append("&timeStamp=" + timeStamp);
			sb.Append("&key=" + _wechatSettings.MerchantSecret);
			var paySign = Crypto.MD5(sb.ToString()).ToUpper();

			return new WeChatJsapiPayParameter {
				timestamp = timeStamp,
				nonceStr = nonceStr,
				package = $"prepay_id={prepayId}",
				signType = "MD5",
				paySign = paySign
			};
		}

		public WeChatJsapiPayParameter JsapiPayCreateParameter(WeChatJsapiPayModel model) {
			_wechatSettings.RequireMobilePlatformSettings();
			_wechatSettings.RequireMerchantSettings();

			var apiUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder";

			var now = DateTime.Now;
			var nonceStr = Crypto.RandomString(32);

			var dictionary = new Dictionary<string, string> {
				{ "appid", _wechatSettings.MobilePlatformAppId! },
				{ "mch_id", _wechatSettings.MerchantId! },
				{ "device_info", "WEB" },
				{ "nonce_str", nonceStr },
				{ "sign_type", "MD5" },
				{ "body", model.Body ?? string.Empty },
				{ "out_trade_no", model.InternalOrderId },
				{ "total_fee", (model.Amount * 100).ToString("f0") },
				{ "spbill_create_ip", _http.Connection.RemoteIpAddress.ToString() },
				{ "time_start", now.ToString("yyyyMMddHHmmss") },
				{ "time_expire", now.Add(model.Expiration).ToString("yyyyMMddHHmmss") },
				{ "notify_url", model.NotifyUrl },
				{ "trade_type", "JSAPI" },
				{ "openid", model.OpenId },
				{ "attach", model.Attach ?? Crypto.MD5(model.InternalOrderId + model.Amount) }
			};

			var response = JsapiPayGetApiResultXml(apiUrl, dictionary);
			var prepayId = response.MidBy("<prepay_id><![CDATA[", "]]></prepay_id>");

			return JsapiPayCreateParameter(prepayId!);
		}

		public bool JsapiPayQueryOrder(string internalOrderId, decimal amount) {
			_wechatSettings.RequireMobilePlatformSettings();
			_wechatSettings.RequireMerchantSettings();

			var apiUrl = "https://api.mch.weixin.qq.com/pay/orderquery";

			var nonceStr = Crypto.RandomString(32);
			var dictionary = new Dictionary<string, string> {
				{ "appid", _wechatSettings.MobilePlatformAppId! },
				{ "mch_id", _wechatSettings.MerchantId! },
				{ "nonce_str", nonceStr },
				{ "out_trade_no", internalOrderId },
				{ "sign_type", "MD5" },
			};

			var response = JsapiPayGetApiResultXml(apiUrl, dictionary);
			var totalFee = response.MidBy("<total_fee>", "</total_fee>");
			var tradeState = response.MidBy("<trade_state><![CDATA[", "]]></trade_state>");

			return tradeState == "SUCCESS" && totalFee == (amount * 100).ToString("f0");
		}

		public string JsapiPayGetApiResultXml(string wechatApiUrl, Dictionary<string, string> parameters) {
			_wechatSettings.RequireMobilePlatformSettings();
			_wechatSettings.RequireMerchantSettings();

			var sb = new StringBuilder();

			var orderedNames = parameters.Keys.OrderBy(x => x).ToArray();
			foreach ( var name in orderedNames ) {
				sb.Append(name + "=" + parameters[name] + "&");
			}
			sb.Append("key=" + _wechatSettings.MerchantSecret);

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
