using System.Net;
using Newtonsoft.Json;

namespace Husky.WeChatIntegration.ServiceCategorized
{
	public class WeChatUserService
	{
		public WeChatUserService(WeChatAppConfig wechatConfig) {
			_wechatConfig = wechatConfig;
		}

		private readonly WeChatAppConfig _wechatConfig;

		public WeChatUserResult? GetUserInfo(WeChatUserAccessToken token) {
			return GetUserInfo(token.OpenId, token.AccessToken);
		}
		public WeChatUserResult? GetUserInfo(string openId, string accessToken) {
			var url = $"https://api.weixin.qq.com/sns/userinfo" + $"?access_token={accessToken}&openid={openId}&lang=zh-CN";
			using ( var client = new WebClient() ) {
				var json = client.DownloadString(url);
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
		}

		public WeChatUserAccessToken? GetOpenPlatformUserAccessToken(string code) {
			_wechatConfig.RequireOpenPlatformSettings();

			return GetUserAccessToken(code, new WeChatAppIdSecret {
				AppId = _wechatConfig.OpenPlatformAppId,
				AppSecret = _wechatConfig.OpenPlatformAppSecret
			});
		}
		public WeChatUserAccessToken? GetMobilePlatformUserAccessToken(string code) {
			_wechatConfig.RequireMobilePlatformSettings();

			return GetUserAccessToken(code, new WeChatAppIdSecret {
				AppId = _wechatConfig.MobilePlatformAppId,
				AppSecret = _wechatConfig.MobilePlatformAppSecret
			});
		}
		public WeChatUserAccessToken? GetUserAccessToken(string code, WeChatAppIdSecret overrideIdSecret) {
			overrideIdSecret.CheckNull();

			var url = $"https://api.weixin.qq.com/sns/oauth2/access_token" +
					  $"?appid={overrideIdSecret.AppId}&secret={overrideIdSecret.AppSecret}&code={code}&grant_type=authorization_code";
			return GetUserAccessTokenFromResolvedUrl(url);
		}


		public WeChatUserAccessToken? RefreshOpenPlatformUserAccessToken(string refreshToken) {
			_wechatConfig.RequireOpenPlatformSettings();

			return RefreshUserAccessToken(refreshToken, new WeChatAppIdSecret {
				AppId = _wechatConfig.OpenPlatformAppId,
				AppSecret = _wechatConfig.OpenPlatformAppSecret
			});
		}
		public WeChatUserAccessToken? RefreshMobilePlatformUserAccessToken(string refreshToken) {
			_wechatConfig.RequireMobilePlatformSettings();

			return RefreshUserAccessToken(refreshToken, new WeChatAppIdSecret {
				AppId = _wechatConfig.MobilePlatformAppId,
				AppSecret = _wechatConfig.MobilePlatformAppSecret
			});
		}
		public WeChatUserAccessToken? RefreshUserAccessToken(string refreshToken, WeChatAppIdSecret overrideIdSecret) {
			overrideIdSecret.CheckNull();

			var url = $"https://api.weixin.qq.com/sns/oauth2/refresh_token" +
					  $"?appid={overrideIdSecret.AppId}&refresh_token={refreshToken}&grant_type=refresh_token";
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
	}
}
