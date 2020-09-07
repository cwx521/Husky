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
			_http = http.HttpContext;
			_cache = cache;
			_wechatSettings = wechatSettings;
		}

		private readonly HttpContext _http;
		private readonly IMemoryCache _cache;
		private readonly WeChatAppSettings _wechatSettings;

		public string CreateLoginQrCode(string redirectUri, string styleSheetUrl, WeChatAppIdSecret overrideAppIdSecret = null) {
			var idSecret = overrideAppIdSecret ?? new WeChatAppIdSecret {
				AppId = _wechatSettings.OpenPlatformAppId,
				AppSecret = _wechatSettings.OpenPlatformAppSecret
			};
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
								appid: '" + idSecret.AppId + @"',
								redirect_uri: '" + redirectUri + @"',
								state: '" + Crypto.Encrypt(DateTime.Now.ToString("yyyy-M-d H:mm:ss"), iv: idSecret.AppId) + @"',
								href: '" + styleSheetUrl + @"',
								style: ''
							});
						}		
					})();
				</script>";
			return html;
		}

		public string CreateMobilePlatformAutoLoginSteppingUrl(string redirectUrl, string scope = "snsapi_userinfo", WeChatAppIdSecret overrideAppIdSecret = null) {
			var idSecret = overrideAppIdSecret ?? new WeChatAppIdSecret {
				AppId = _wechatSettings.MobilePlatformAppId,
				AppSecret = _wechatSettings.MobilePlatformAppSecret
			};
			return $"https://open.weixin.qq.com/connect/oauth2/authorize" +
				   $"?appid={idSecret.AppId}" +
				   $"&redirect_uri={HttpUtility.UrlEncode(redirectUrl)}" +
				   $"&response_type=code" +
				   $"&scope={scope}" +
				   $"&state={Crypto.Encrypt(DateTime.Now.ToString("yyyy-M-d H:mm:ss"), iv: idSecret.AppId)}" +
				   $"#wechat_redirect";
		}

		public WeChatUserAccessToken GetOpenPlatformUserAccessToken(string code, WeChatAppIdSecret overrideAppIdSecret = null) {
			return GetUserAccessToken(code, overrideAppIdSecret ?? new WeChatAppIdSecret {
				AppId = _wechatSettings.OpenPlatformAppId,
				AppSecret = _wechatSettings.OpenPlatformAppSecret
			});
		}
		public WeChatUserAccessToken GetMobilePlatformUserAccessToken(string code, WeChatAppIdSecret overrideAppIdSecret = null) {
			return GetUserAccessToken(code, overrideAppIdSecret ?? new WeChatAppIdSecret {
				AppId = _wechatSettings.MobilePlatformAppId,
				AppSecret = _wechatSettings.MobilePlatformAppSecret
			});
		}
		public WeChatUserAccessToken GetUserAccessToken(string code, WeChatAppIdSecret idSecret) {
			var url = $"https://api.weixin.qq.com/sns/oauth2/access_token" +
					  $"?appid={idSecret.AppId}&secret={idSecret.AppSecret}&code={code}&grant_type=authorization_code";
			return GetUserAccessTokenFromResolvedUrl(url);
		}

		public WeChatUserAccessToken RefreshOpenPlatformUserAccessToken(string refreshToken, WeChatAppIdSecret overrideAppIdSecret = null) {
			return RefreshUserAccessToken(refreshToken, overrideAppIdSecret ?? new WeChatAppIdSecret {
				AppId = _wechatSettings.OpenPlatformAppId,
				AppSecret = _wechatSettings.OpenPlatformAppSecret
			});
		}
		public WeChatUserAccessToken RefreshMobilePlatformUserAccessToken(string refreshToken, WeChatAppIdSecret overrideAppIdSecret = null) {
			return RefreshUserAccessToken(refreshToken, overrideAppIdSecret ?? new WeChatAppIdSecret {
				AppId = _wechatSettings.MobilePlatformAppId,
				AppSecret = _wechatSettings.MobilePlatformAppSecret
			});
		}
		public WeChatUserAccessToken RefreshUserAccessToken(string refreshToken, WeChatAppIdSecret idSecret) {
			var url = $"https://api.weixin.qq.com/sns/oauth2/refresh_token" +
					  $"?appid={idSecret.AppId}&refresh_token={refreshToken}&grant_type=refresh_token";
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

		public WeChatMiniProgramLoginResult MiniProgramLogin(string code, WeChatAppIdSecret overrideAppIdSecret = null) {
			var idSecret = overrideAppIdSecret ?? new WeChatAppIdSecret {
				AppId = _wechatSettings.MiniProgramAppId,
				AppSecret = _wechatSettings.MiniProgramAppSecret
			};
			var url = $"https://api.weixin.qq.com/sns/jscode2session" +
					  $"?appid={idSecret.AppId}" +
					  $"&secret={idSecret.AppSecret}" +
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

		public WeChatGeneralAccessToken GetMobilePlatformGeneralAccessToken(WeChatAppIdSecret overrideAppIdSecret = null) {
			var idSecret = overrideAppIdSecret ?? new WeChatAppIdSecret {
				AppId = _wechatSettings.MobilePlatformAppId,
				AppSecret = _wechatSettings.MobilePlatformAppSecret
			};
			return _cache.GetOrCreate(idSecret.AppId + nameof(GetMobilePlatformGeneralAccessToken), entry => {
				var url = $"https://api.weixin.qq.com/cgi-bin/token" +
					  $"?grant_type=client_credential" +
					  $"&appid={idSecret.AppId}" +
					  $"&secret={idSecret.AppSecret}";

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

		public string GetJsapiTicket(WeChatAppIdSecret overrideAppIdSecret = null) {
			var idSecret = overrideAppIdSecret ?? new WeChatAppIdSecret {
				AppId = _wechatSettings.MobilePlatformAppId,
				AppSecret = _wechatSettings.MobilePlatformAppSecret
			};
			return _cache.GetOrCreate(idSecret.AppId + nameof(GetJsapiTicket), entry => {
				var accessToken = GetMobilePlatformGeneralAccessToken(idSecret);
				var url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket" + $"?access_token={accessToken.AccessToken}&type=jsapi";

				using ( var client = new WebClient() ) {
					var json = client.DownloadString(url);
					var d = JsonConvert.DeserializeObject<dynamic>(json);
					entry.SetAbsoluteExpiration(TimeSpan.FromSeconds((int)d.expires_in));
					return d.ticket;
				}
			});
		}

		public WeChatJsapiConfig BuildWeChatJsapiConfig(WeChatAppIdSecret overrideAppIdSecret = null) {
			var idSecret = overrideAppIdSecret ?? new WeChatAppIdSecret {
				AppId = _wechatSettings.MobilePlatformAppId,
				AppSecret = _wechatSettings.MobilePlatformAppSecret
			};
			var config = new WeChatJsapiConfig {
				AppId = idSecret.AppId,
				NonceStr = Crypto.RandomString(16),
				Timestamp = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).Ticks / 1000,
				Ticket = GetJsapiTicket(idSecret),
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
								jsApiList: [" + string.Join(',', jsApiList.Select(x => $"'{x}'")) + @"]
							});
						}
					}
					setTimeout(loadWeChatConfig, 50);
				</script>";
		}

		public WeChatJsapiPayParameter BuildMobilePlatformJsapiPayParameter(string prepayId, WeChatAppIdSecret overrideAppIdSecret = null) {
			var idSecret = overrideAppIdSecret ?? new WeChatAppIdSecret {
				AppId = _wechatSettings.MobilePlatformAppId,
				AppSecret = _wechatSettings.MobilePlatformAppSecret
			};
			var nonceStr = Crypto.RandomString(32);
			var timeStamp = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).Ticks / 1000;

			var sb = new StringBuilder();
			sb.Append("appId=" + idSecret.AppId);
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

		public string GetApiResultXml(string wechatApiUrl, Dictionary<string, string> parameters, string overrideMerchantSecret = null) {
			var sb = new StringBuilder();

			var orderedNames = parameters.Keys.OrderBy(x => x).ToArray();
			foreach ( var name in orderedNames ) {
				sb.Append(name + "=" + parameters[name] + "&");
			}
			sb.Append("key=" + overrideMerchantSecret ?? _wechatSettings.MerchantSecret);

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
