using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Husky.WeChatIntegration.ServiceCategorized
{
	public class WeChatMobilePlatformService
	{
		public WeChatMobilePlatformService(WeChatOptions options, IHttpContextAccessor httpContextAccessor, IMemoryCache cache) {
			_options = options;
			_httpContextAccessor = httpContextAccessor;
			_cache = cache;
		}

		private readonly WeChatOptions _options;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IMemoryCache _cache;

		#region 公众号内微信身份登录，包装登录跳转地址，获取Code (Code在跳转后地址的QueryString中)
		public string CreateAutoLoginUrl(WeChatAppIdSecret appIdSecret, string redirectUrl, string scope = "snsapi_userinfo") {
			appIdSecret.NotNull();
			appIdSecret.MustBe(WeChatAppRegion.MobilePlatform);

			return $"https://open.weixin.qq.com/connect/oauth2/authorize" +
				   $"?appid={appIdSecret.AppId}" +
				   $"&redirect_uri={HttpUtility.UrlEncode(redirectUrl)}" +
				   $"&response_type=code" +
				   $"&scope={scope}" +
				   $"&state={Crypto.Encrypt(DateTime.Now.ToString("yyyy-M-d H:mm:ss"), iv: appIdSecret.AppId!)}" +
				   $"#wechat_redirect";
		}
		#endregion

		#region 获取 UserAccessToken (code 换取 OpenId)
		public async Task<Result<WeChatUserAccessToken>> GetUserAccessTokenAsync(WeChatAppIdSecret appIdSecret, string code) {
			appIdSecret.NotNull();
			appIdSecret.MustHaveSecret();
			appIdSecret.MustBe(WeChatAppRegion.MobilePlatform);

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
						OpenId = d.openid,
						UnionId = d.unionid
					}
				};
			}
			catch (Exception e) {
				return new Failure<WeChatUserAccessToken>(e.Message);
			}
		}
		#endregion

		#region 获取用户信息		

		public async Task<Result<WeChatUserInfoResult>> GetUserInfoAsync(WeChatUserAccessToken userAccessToken) {
			try {
				var url = $"https://api.weixin.qq.com/sns/userinfo" + $"?access_token={userAccessToken.AccessToken}&openid={userAccessToken.OpenId}&lang=zh_CN";
				var json = await WeChatService.HttpClient.GetStringAsync(url);
				var d = JsonConvert.DeserializeObject<dynamic>(json)!;

				if (d.errcode != null && (int)d.errcode != 0) {
					return new Failure<WeChatUserInfoResult>((int)d.errcode + ": " + d.errmsg);
				}
				return new Success<WeChatUserInfoResult> {
					Data = new WeChatUserInfoResult {
						OpenId = d.openid,
						UnionId = d.unionid,
						NickName = d.nickname,
						Sex = d.sex == 2 ? Sex.Female : Sex.Male,
						Province = d.province,
						City = d.city,
						Country = d.country,
						HeadImageUrl = ((string)d.headimgurl).Replace("http://", "https://")
					}
				};
			}
			catch (Exception e) {
				return new Failure<WeChatUserInfoResult>(e.Message);
			}
		}

		public async Task<Result<WeChatUserSubscriptionStatusResult>> GetUserSubscriptionStatus(WeChatUserAccessToken userAccessToken) {
			try {
				var url = $"https://api.weixin.qq.com/cgi-bin/user/info" + $"?access_token={userAccessToken.AccessToken}&openid={userAccessToken.OpenId}&lang=zh_CN";
				var json = await WeChatService.HttpClient.GetStringAsync(url);
				var d = JsonConvert.DeserializeObject<dynamic>(json)!;

				if (d.errcode != null && (int)d.errcode != 0) {
					return new Failure<WeChatUserSubscriptionStatusResult>((int)d.errcode + ": " + d.errmsg);
				}
				return new Success<WeChatUserSubscriptionStatusResult> {
					Data = new WeChatUserSubscriptionStatusResult {
						Subscribed = d.subscribe != null && (int)d.subscribe == 1,
						SubscribeTime = d.subscribe == null || (int)d.subscribe != 1 ? null : new DateTime(1970, 1, 1).AddSeconds((int)d.subscribe_time),
						SubscribeScene = d.subscribe_scene,
						OpenId = d.openid,
						UnionId = d.unionid,
					}
				};
			}
			catch (Exception e) {
				return new Failure<WeChatUserSubscriptionStatusResult>(e.Message);
			}
		}

		#endregion

		#region JsApi相关

		private async Task<string> GetJsApiTicketAsync(WeChatGeneralAccessToken accessToken) {
			return await _cache.GetOrCreate(accessToken.AccessToken + nameof(GetJsApiTicketAsync), async entry => {
				var url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket" + $"?access_token={accessToken.AccessToken}&type=jsapi";
				try {
					var json = await WeChatService.HttpClient.GetStringAsync(url);
					var d = JsonConvert.DeserializeObject<dynamic>(json)!;

					var ok = d.errcode == null || (int)d.errcode == 0;
					entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(ok ? (int)d.expires_in : 1));

					return d.ticket ?? ((int)d.errcode + ": " + d.errmsg);
				}
				catch (Exception e) {
					return e.Message;
				}
			})!;
		}

		public WeChatJsApiConfig GetJsApiConfig(WeChatAppIdSecret appIdSecret, string jsApiTicket) {
			if (_httpContextAccessor.HttpContext == null) {
				throw new InvalidProgramException("Can not call this method when IHttpContextAccessor.HttpContext is null.");
			}

			var config = new WeChatJsApiConfig {
				AppId = appIdSecret.AppId!,
				NonceStr = Crypto.RandomString(16),
				Timestamp = DateTime.Now.Timestamp(),
				Ticket = jsApiTicket,
			};

			var sb = new StringBuilder();
			sb.Append("jsapi_ticket=" + config.Ticket);
			sb.Append("&noncestr=" + config.NonceStr);
			sb.Append("&timestamp=" + config.Timestamp.ToString());
			sb.Append("&url=" + _httpContextAccessor.HttpContext.Request.FullUrl().Split('#').First());

			config.RawString = sb.ToString();
			config.Signature = Crypto.SHA1(config.RawString);
			return config;
		}

		public const string JsSdkUrl = "https://res.wx.qq.com/open/js/jweixin-1.6.0.js";
		public const string JsSdkUrlAlternate = "https://res2.wx.qq.com/open/js/jweixin-1.6.0.js";
		public const string DefaultEnabledJsApiNames = "updateAppMessageShareData,updateTimelineShareData,onMenuShareAppMessage,onMenuShareTimeline,openLocation,getLocation,scanQRCode,chooseWXPay,getNetworkType,chooseImage,previewImage,hideMenuItems,closWindow";
		public const string DefaultEnabledOpenTags = "wx-open-subscribe";
		public string CreateJsSdkReferenceScript() => $"<script src=\"{JsSdkUrl}\"></script>";
		public string CreateJsApiConfigScript(WeChatAppIdSecret appIdSecret, string jsApiTicket, bool withScriptTag = true, string enableJsApiNames = DefaultEnabledJsApiNames, string enableOpenTags = DefaultEnabledOpenTags) {
			var cfg = GetJsApiConfig(appIdSecret, jsApiTicket);
			var script = @"
				function configWeChatJsApi() {
					if (typeof(wx) == undefined) {
						setTimeout(configWeChatJsApi, 50);
					}
					else {
						wx.config({
							debug: false,
							appId: '" + cfg.AppId + @"',
							timestamp: " + cfg.Timestamp + @",
							nonceStr: '" + cfg.NonceStr + @"',
							signature: '" + cfg.Signature + @"',
							jsApiList: [" + string.Join(',', enableJsApiNames.Split(',', '|').Select(x => $"'{x.Trim()}'")) + @"],
							openTagList: [" + string.Join(',', enableOpenTags.Split(',', '|').Select(x => $"'{x.Trim()}'")) + @"]
						});
					}
				}
				setTimeout(configWeChatJsApi, 50);";

			return !withScriptTag ? script : $"<script type='text/javascript'>\r\n{script}\r\n</script>";
		}

		#endregion

		#region 发送模板消息和订阅消息

		public async Task<Result> SendTemplateMessageAsync<T>(WeChatGeneralAccessToken accessToken, string openId, T data, string? url) where T : IWeChatTemplateMessage {
			var api = "https://" + $"api.weixin.qq.com/cgi-bin/message/template/send?access_token={accessToken.AccessToken}";

			var message = new {
				touser = openId,
				template_id = data.TemplateId,
				url = url,
				topcolor = "#FF6600",
				data = data
			};
			var serialized = System.Text.Json.JsonSerializer.Serialize((object)message, new JsonSerializerOptions(JsonSerializerDefaults.Web));

			var response = await WeChatService.HttpClient.PostAsync(api, new StringContent(serialized));
			var json = await response.Content.ReadAsStringAsync();
			var d = JsonConvert.DeserializeObject<dynamic>(json)!;

			var ok = d.errcode == null || (int)d.errcode == 0;
			if (!ok) {
				return new Failure((int)d.errcode + ": " + d.errmsg);
			}
			return new Success();
		}

		public async Task<Result> SendSubscribedMessageAsync<T>(WeChatGeneralAccessToken accessToken, string openId, T data, string? url) where T : IWeChatTemplateMessage {
			var api = "https://" + $"api.weixin.qq.com/cgi-bin/message/subscribe/bizsend?access_token={accessToken.AccessToken}";

			var message = new {
				touser = openId,
				template_id = data.TemplateId,
				page = url,
				data = data
			};
			var serialized = System.Text.Json.JsonSerializer.Serialize((object)message, new JsonSerializerOptions(JsonSerializerDefaults.Web));

			var response = await WeChatService.HttpClient.PostAsync(api, new StringContent(serialized));
			var json = await response.Content.ReadAsStringAsync();
			var d = JsonConvert.DeserializeObject<dynamic>(json)!;

			var ok = d.errcode == null || (int)d.errcode == 0;
			if (!ok) {
				return new Failure((int)d.errcode + ": " + d.errmsg);
			}
			return new Success();
		}

		#endregion
	}
}
