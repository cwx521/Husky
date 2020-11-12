using System;
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

		public async Task<Result<WeChatUserResult>> GetUserInfoAsync(WeChatUserAccessToken token) => await GetUserInfoAsync(token.OpenId, token.AccessToken);
		public async Task<Result<WeChatUserResult>> GetUserInfoAsync(string openId, string accessToken) {
			try {
				var url = $"https://api.weixin.qq.com/sns/userinfo" + $"?access_token={accessToken}&openid={openId}&lang=zh-CN";
				var json = await DefaultHttpClient.Instance.GetStringAsync(url);
				var d = JsonConvert.DeserializeObject<dynamic>(json);

				if ( d.errcode != null && (int)d.errcode != 0 ) {
					return new Failure<WeChatUserResult>(d.errmsg);
				}

				return new Success<WeChatUserResult> {
					Data = new WeChatUserResult {
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
			catch ( Exception e ) {
				return new Failure<WeChatUserResult>(e.Message);
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

		private async Task<Result<WeChatUserAccessToken>> GetUserAccessTokenFromResolvedUrlAsync(string url) {
			try {
				var json = await DefaultHttpClient.Instance.GetStringAsync(url);
				var d = JsonConvert.DeserializeObject<dynamic>(json);

				if ( d.errcode != null && (int)d.errcode != 0 ) {
					return new Failure<WeChatUserAccessToken>(d.errmsg);
				}

				return new Success<WeChatUserAccessToken> {
					Data = new WeChatUserAccessToken {
						AccessToken = d.access_token,
						RefreshToken = d.refresh_token,
						OpenId = d.openid
					}
				};
			}
			catch ( Exception e ) {
				return new Failure<WeChatUserAccessToken>(e.Message);
			}
		}
	}
}
