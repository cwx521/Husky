using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Husky.WeChatIntegration.ServiceCategorized
{
	public class WeChatUserService
	{
		public WeChatUserService(WeChatOptions options) {
			_options = options;
		}

		private readonly WeChatOptions _options;


		public async Task<Result<WeChatUserSubscriptionStatusResult>> GetUserSubscriptionStatus(WeChatUserAccessToken token) => await GetUserSubscriptionStatus(token.OpenId, token.AccessToken);

		public async Task<Result<WeChatUserSubscriptionStatusResult>> GetUserSubscriptionStatus(string openId, string accessToken) {
			try {
				var url = $"https://api.weixin.qq.com/cgi-bin/user/info" + $"?access_token={accessToken}&openid={openId}&lang=zh_CN";
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

		public async Task<Result<WeChatUserInfoResult>> GetUserInfoAsync(WeChatUserAccessToken token) => await GetUserInfoAsync(token.OpenId, token.AccessToken);

		public async Task<Result<WeChatUserInfoResult>> GetUserInfoAsync(string openId, string accessToken) {
			try {
				var url = $"https://api.weixin.qq.com/sns/userinfo" + $"?access_token={accessToken}&openid={openId}&lang=zh_CN";
				var json = await WeChatService.HttpClient.GetStringAsync(url);
				var d = JsonConvert.DeserializeObject<dynamic>(json)!;

				if (d.errcode != null && (int)d.errcode != 0) {
					return new Failure<WeChatUserInfoResult>((int)d.errcode + ": " + d.errmsg);
				}
				return new Success<WeChatUserInfoResult> {
					Data = new WeChatUserInfoResult {
						Subscribed = d.subscribe != null && (int)d.subscribe == 1,
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

		public async Task<Result<WeChatUserAccessToken>> GetOpenPlatformUserAccessTokenAsync(string code) {
			_options.RequireOpenPlatformSettings();

			return await GetUserAccessTokenAsync(code, new WeChatAppIdSecret {
				AppId = _options.OpenPlatformAppId,
				AppSecret = _options.OpenPlatformAppSecret
			});
		}
		public async Task<Result<WeChatUserAccessToken>> GetMobilePlatformUserAccessTokenAsync(string code) {
			_options.RequireMobilePlatformSettings();

			return await GetUserAccessTokenAsync(code, new WeChatAppIdSecret {
				AppId = _options.MobilePlatformAppId,
				AppSecret = _options.MobilePlatformAppSecret
			});
		}

		[SuppressMessage("Performance", "CA1822:Mark members as static")]
		public async Task<Result<WeChatUserAccessToken>> GetUserAccessTokenAsync(string code, WeChatAppIdSecret overrideIdSecret) {
			overrideIdSecret.NotNull();

			var url = $"https://api.weixin.qq.com/sns/oauth2/access_token" +
					  $"?appid={overrideIdSecret.AppId}" +
					  $"&secret={overrideIdSecret.AppSecret}" +
					  $"&code={code}" +
					  $"&grant_type=authorization_code";

			return await GetUserAccessTokenFromResolvedUrlAsync(url);
		}

		public async Task<Result<WeChatUserAccessToken>> RefreshOpenPlatformUserAccessTokenAsync(string refreshToken) {
			_options.RequireOpenPlatformSettings();

			return await RefreshUserAccessTokenAsync(refreshToken, new WeChatAppIdSecret {
				AppId = _options.OpenPlatformAppId,
				AppSecret = _options.OpenPlatformAppSecret
			});
		}
		public async Task<Result<WeChatUserAccessToken>> RefreshMobilePlatformUserAccessTokenAsync(string refreshToken) {
			_options.RequireMobilePlatformSettings();

			return await RefreshUserAccessTokenAsync(refreshToken, new WeChatAppIdSecret {
				AppId = _options.MobilePlatformAppId,
				AppSecret = _options.MobilePlatformAppSecret
			});
		}
		public async Task<Result<WeChatUserAccessToken>> RefreshUserAccessTokenAsync(string refreshToken, WeChatAppIdSecret overrideIdSecret) {
			overrideIdSecret.NotNull();

			var url = $"https://api.weixin.qq.com/sns/oauth2/refresh_token" +
					  $"?appid={overrideIdSecret.AppId}" +
					  $"&refresh_token={refreshToken}" +
					  $"&grant_type=refresh_token";

			return await GetUserAccessTokenFromResolvedUrlAsync(url);
		}

		private static async Task<Result<WeChatUserAccessToken>> GetUserAccessTokenFromResolvedUrlAsync(string url) {
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
