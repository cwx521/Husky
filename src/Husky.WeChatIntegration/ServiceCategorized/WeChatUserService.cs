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

		public WeChatUserResult? GetUserInfo(WeChatUserAccessToken token) => GetUserInfo(token.OpenId, token.AccessToken);
		public WeChatUserResult? GetUserInfo(string openId, string accessToken) => GetUserInfoAsync(openId, accessToken).Result;

		public async Task<WeChatUserResult?> GetUserInfoAsync(WeChatUserAccessToken token) => await GetUserInfoAsync(token.OpenId, token.AccessToken);
		public async Task<WeChatUserResult?> GetUserInfoAsync(string openId, string accessToken) {
			var url = $"https://api.weixin.qq.com/sns/userinfo" + $"?access_token={accessToken}&openid={openId}&lang=zh-CN";

			var json = await _httpClient.GetStringAsync(url);
			var d = JsonConvert.DeserializeObject<dynamic>(json);

			if ( d.errcode != null && d.errcode != 0 ) {
				return null;
			}
			return new WeChatUserResult {
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

		public WeChatUserAccessToken? GetOpenPlatformUserAccessToken(string code) => GetOpenPlatformUserAccessTokenAsync(code).Result;
		public WeChatUserAccessToken? GetMobilePlatformUserAccessToken(string code) => GetMobilePlatformUserAccessTokenAsync(code).Result;
		public WeChatUserAccessToken? GetUserAccessToken(string code, WeChatAppIdSecret overrideIdSecret) => GetUserAccessTokenAsync(code, overrideIdSecret).Result;

		public async Task<WeChatUserAccessToken?> GetOpenPlatformUserAccessTokenAsync(string code) {
			_wechatConfig.RequireOpenPlatformSettings();

			return await GetUserAccessTokenAsync(code, new WeChatAppIdSecret {
				AppId = _wechatConfig.OpenPlatformAppId,
				AppSecret = _wechatConfig.OpenPlatformAppSecret
			});
		}
		public async Task<WeChatUserAccessToken?> GetMobilePlatformUserAccessTokenAsync(string code) {
			_wechatConfig.RequireMobilePlatformSettings();

			return await GetUserAccessTokenAsync(code, new WeChatAppIdSecret {
				AppId = _wechatConfig.MobilePlatformAppId,
				AppSecret = _wechatConfig.MobilePlatformAppSecret
			});
		}
		public async Task<WeChatUserAccessToken?> GetUserAccessTokenAsync(string code, WeChatAppIdSecret overrideIdSecret) {
			overrideIdSecret.CheckNull();

			var url = $"https://api.weixin.qq.com/sns/oauth2/access_token" +
					  $"?appid={overrideIdSecret.AppId}" +
					  $"&secret={overrideIdSecret.AppSecret}" +
					  $"&code={code}" +
					  $"&grant_type=authorization_code";

			return await GetUserAccessTokenFromResolvedUrlAsync(url);
		}


		public WeChatUserAccessToken? RefreshOpenPlatformUserAccessToken(string refreshToken) => RefreshOpenPlatformUserAccessTokenAsync(refreshToken).Result;
		public WeChatUserAccessToken? RefreshMobilePlatformUserAccessToken(string refreshToken) => RefreshMobilePlatformUserAccessTokenAsync(refreshToken).Result;
		public WeChatUserAccessToken? RefreshUserAccessToken(string refreshToken, WeChatAppIdSecret overrideIdSecret) => RefreshUserAccessTokenAsync(refreshToken, overrideIdSecret).Result;

		public async Task<WeChatUserAccessToken?> RefreshOpenPlatformUserAccessTokenAsync(string refreshToken) {
			_wechatConfig.RequireOpenPlatformSettings();

			return await RefreshUserAccessTokenAsync(refreshToken, new WeChatAppIdSecret {
				AppId = _wechatConfig.OpenPlatformAppId,
				AppSecret = _wechatConfig.OpenPlatformAppSecret
			});
		}
		public async Task<WeChatUserAccessToken?> RefreshMobilePlatformUserAccessTokenAsync(string refreshToken) {
			_wechatConfig.RequireMobilePlatformSettings();

			return await RefreshUserAccessTokenAsync(refreshToken, new WeChatAppIdSecret {
				AppId = _wechatConfig.MobilePlatformAppId,
				AppSecret = _wechatConfig.MobilePlatformAppSecret
			});
		}
		public async Task<WeChatUserAccessToken?> RefreshUserAccessTokenAsync(string refreshToken, WeChatAppIdSecret overrideIdSecret) {
			overrideIdSecret.CheckNull();

			var url = $"https://api.weixin.qq.com/sns/oauth2/refresh_token" +
					  $"?appid={overrideIdSecret.AppId}" +
					  $"&refresh_token={refreshToken}" +
					  $"&grant_type=refresh_token";

			return await GetUserAccessTokenFromResolvedUrlAsync(url);
		}

		private async Task<WeChatUserAccessToken?> GetUserAccessTokenFromResolvedUrlAsync(string url) {
			var json = await _httpClient.GetStringAsync(url);
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
}
