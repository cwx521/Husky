using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Husky.WeChatIntegration.ServiceCategorized
{
	public class WeChatUserService
	{
		public WeChatUserService(WeChatAppConfig wechatConfig) {
			_wechatConfig = wechatConfig;
		}

		private readonly WeChatAppConfig _wechatConfig;
		private static readonly HttpClient _httpClient = new HttpClient();

		public async Task<WeChatUserResult> GetUserInfoAsync(WeChatUserAccessToken token) => await GetUserInfoAsync(token.OpenId, token.AccessToken);
		public async Task<WeChatUserResult> GetUserInfoAsync(string openId, string accessToken) {
			var url = $"https://api.weixin.qq.com/sns/userinfo" + $"?access_token={accessToken}&openid={openId}&lang=zh-CN";

			try {
				var json = await _httpClient.GetStringAsync(url);
				var d = JsonConvert.DeserializeObject<dynamic>(json);

				return new WeChatUserResult {
					Ok = d.errcode == null || (int)d.errcode == 0,
					Message = d.errmsg,

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
			catch ( Exception e ) {
				return new WeChatUserResult {
					Ok = false,
					Message = e.Message
				};
			}
		}

		public async Task<WeChatUserAccessToken> GetOpenPlatformUserAccessTokenAsync(string code) {
			_wechatConfig.RequireOpenPlatformSettings();

			return await GetUserAccessTokenAsync(code, new WeChatAppIdSecret {
				AppId = _wechatConfig.OpenPlatformAppId,
				AppSecret = _wechatConfig.OpenPlatformAppSecret
			});
		}
		public async Task<WeChatUserAccessToken> GetMobilePlatformUserAccessTokenAsync(string code) {
			_wechatConfig.RequireMobilePlatformSettings();

			return await GetUserAccessTokenAsync(code, new WeChatAppIdSecret {
				AppId = _wechatConfig.MobilePlatformAppId,
				AppSecret = _wechatConfig.MobilePlatformAppSecret
			});
		}
		public async Task<WeChatUserAccessToken> GetUserAccessTokenAsync(string code, WeChatAppIdSecret overrideIdSecret) {
			overrideIdSecret.CheckNull();

			var url = $"https://api.weixin.qq.com/sns/oauth2/access_token" +
					  $"?appid={overrideIdSecret.AppId}" +
					  $"&secret={overrideIdSecret.AppSecret}" +
					  $"&code={code}" +
					  $"&grant_type=authorization_code";

			return await GetUserAccessTokenFromResolvedUrlAsync(url);
		}

		public async Task<WeChatUserAccessToken> RefreshOpenPlatformUserAccessTokenAsync(string refreshToken) {
			_wechatConfig.RequireOpenPlatformSettings();

			return await RefreshUserAccessTokenAsync(refreshToken, new WeChatAppIdSecret {
				AppId = _wechatConfig.OpenPlatformAppId,
				AppSecret = _wechatConfig.OpenPlatformAppSecret
			});
		}
		public async Task<WeChatUserAccessToken> RefreshMobilePlatformUserAccessTokenAsync(string refreshToken) {
			_wechatConfig.RequireMobilePlatformSettings();

			return await RefreshUserAccessTokenAsync(refreshToken, new WeChatAppIdSecret {
				AppId = _wechatConfig.MobilePlatformAppId,
				AppSecret = _wechatConfig.MobilePlatformAppSecret
			});
		}
		public async Task<WeChatUserAccessToken> RefreshUserAccessTokenAsync(string refreshToken, WeChatAppIdSecret overrideIdSecret) {
			overrideIdSecret.CheckNull();

			var url = $"https://api.weixin.qq.com/sns/oauth2/refresh_token" +
					  $"?appid={overrideIdSecret.AppId}" +
					  $"&refresh_token={refreshToken}" +
					  $"&grant_type=refresh_token";

			return await GetUserAccessTokenFromResolvedUrlAsync(url);
		}

		private async Task<WeChatUserAccessToken> GetUserAccessTokenFromResolvedUrlAsync(string url) {
			try {
				var json = await _httpClient.GetStringAsync(url);
				var d = JsonConvert.DeserializeObject<dynamic>(json);

				return new WeChatUserAccessToken {
					Ok = d.errcode == null || (int)d.errcode == 0,
					Message = d.errmsg,

					AccessToken = d.access_token,
					RefreshToken = d.refresh_token,
					OpenId = d.openid
				};
			}
			catch ( Exception e ) {
				return new WeChatUserAccessToken {
					Ok = false,
					Message = e.Message
				};
			}
		}
	}
}
