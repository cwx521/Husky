using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Husky.WeChatIntegration.ServiceCategorized
{
	public class WeChatJsApiService
	{
		public WeChatJsApiService(WeChatOptions options, IHttpContextAccessor httpContextAccessor, IMemoryCache cache) {
			_options = options;
			_httpContextAccessor = httpContextAccessor;
			_cache = cache;
		}

		private readonly WeChatOptions _options;
		private readonly IMemoryCache _cache;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public async Task<Result<WeChatGeneralAccessToken>> GetGeneralAccessTokenAsync() {
			_options.RequireMobilePlatformSettings();

			return await _cache.GetOrCreate<Task<Result<WeChatGeneralAccessToken>>>(_options.MobilePlatformAppId + nameof(GetGeneralAccessTokenAsync), async entry => {
				var url = _options.GeneralAccessTokenManagedCentral ?? "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential";
				url += url.Contains('?') ? '&' : '?';
				url += $"appid={_options.MobilePlatformAppId}&secret={_options.MobilePlatformAppSecret}";

				try {
					var json = await DefaultHttpClient.Instance.GetStringAsync(url);
					var d = JsonConvert.DeserializeObject<dynamic>(json);

					var ok = d.errcode == null || (int)d.errcode == 0;
					if ( !ok ) {
						entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(1));
						return new Failure<WeChatGeneralAccessToken>((int)d.errcode + ": " + d.errmsg);
					}

					var timeSpan = d.expires_at != null
						? ((DateTime)d.expires_at).Subtract(DateTime.Now)
						: TimeSpan.FromSeconds((int)d.expires_in);

					entry.SetAbsoluteExpiration(timeSpan);

					return new Success<WeChatGeneralAccessToken> {
						Data = new WeChatGeneralAccessToken {
							AccessToken = d.access_token,
							Expires = DateTime.Now.Add(timeSpan)
						}
					};
				}
				catch ( Exception e ) {
					return new Failure<WeChatGeneralAccessToken>(e.Message);
				}
			});
		}

		public void RemoveGeneralAccessTokenCache() {
			_cache.Remove(_options.MobilePlatformAppId + nameof(GetGeneralAccessTokenAsync));
		}

		public async Task<string> GetJsApiTicketAsync(WeChatGeneralAccessToken accessToken) {
			return await _cache.GetOrCreate(accessToken.AccessToken + nameof(GetJsApiTicketAsync), async entry => {
				var url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket" + $"?access_token={accessToken.AccessToken}&type=jsapi";
				try {
					var json = await DefaultHttpClient.Instance.GetStringAsync(url);
					var d = JsonConvert.DeserializeObject<dynamic>(json);

					var ok = d.errcode == null || (int)d.errcode == 0;
					entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(ok ? (int)d.expires_in : 1));

					return d.ticket ?? ((int)d.errcode + ": " + d.errmsg);
				}
				catch ( Exception e ) {
					return e.Message;
				}
			});
		}

		public async Task<WeChatJsApiConfig> CreateJsApiConfigAsync(WeChatGeneralAccessToken accessToken) {
			if ( _httpContextAccessor.HttpContext == null ) {
				throw new InvalidProgramException("Can not call this method when IHttpContextAccessor.HttpContext is null.");
			}

			_options.RequireMobilePlatformSettings();

			var config = new WeChatJsApiConfig {
				AppId = _options.MobilePlatformAppId!,
				NonceStr = Crypto.RandomString(16),
				Timestamp = DateTime.Now.Timestamp(),
				Ticket = await GetJsApiTicketAsync(accessToken),
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

		public string CreateJsSdkReferenceScript() {
			return $"<script src=\"{JsSdkUrl}\"></script>";
		}

		private const string _defaultEnabledJsApiNames = "updateAppMessageShareData,updateTimelineShareData,onMenuShareAppMessage,onMenuShareTimeline,openLocation,getLocation,scanQRCode,chooseWXPay,getNetworkType,chooseImage,previewImage,hideMenuItems,closWindow";
		private const string _defaultEnabledOpenTags = "wx-open-subscribe";
		public async Task<string> CreateJsApiConfigScriptAsync(WeChatGeneralAccessToken accessToken, bool withScriptTag = true, string enableJsApiNames = _defaultEnabledJsApiNames, string enableOpenTags = _defaultEnabledOpenTags) {
			var cfg = await CreateJsApiConfigAsync(accessToken);
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

			if ( !withScriptTag ) {
				return script;
			}
			return $"<script type='text/javascript'>\r\n{script}\r\n</script>";
		}

		public async Task<Result> SendTemplateMessageAsync<T>(WeChatGeneralAccessToken accessToken, string openId, T data, string? url) where T : IWeChatTemplateMessage {
			var api = "https://" + $"api.weixin.qq.com/cgi-bin/message/template/send?access_token={accessToken.AccessToken}";

			var message = new {
				touser = openId,
				template_id = data.TemplateId,
				url = url,
				page = url,
				topcolor = "#FF6600",
				data = data
			};
			var serialized = System.Text.Json.JsonSerializer.Serialize((object)message, new JsonSerializerOptions(JsonSerializerDefaults.Web));

			var response = await DefaultHttpClient.Instance.PostAsync(api, new StringContent(serialized));
			var json = await response.Content.ReadAsStringAsync();
			var d = JsonConvert.DeserializeObject<dynamic>(json);

			var ok = d.errcode == null || (int)d.errcode == 0;
			if ( !ok ) {
				return new Failure<WeChatGeneralAccessToken>((int)d.errcode + ": " + d.errmsg);
			}
			return new Success();
		}
	}
}
