using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Husky.WeChatIntegration.ServiceCategorized
{
	public class WeChatMiniProgramService
	{
		public async Task<Result<WeChatMiniProgramLoginResult>> ProceedLoginAsync(WeChatAppIdSecret appIdSecret, string code) {
			appIdSecret.NotNull();
			appIdSecret.MustHaveSecret();
			appIdSecret.MustBe(WeChatAppRegion.MiniProgram);

			var url = $"https://api.weixin.qq.com/sns/jscode2session" +
					  $"?appid={appIdSecret.AppId}" +
					  $"&secret={appIdSecret.AppSecret}" +
					  $"&js_code={code}" +
					  $"&grant_type=authorization_code";

			try {
				var json = await WeChatService.HttpClient.GetStringAsync(url);
				var d = JsonConvert.DeserializeObject<dynamic>(json)!;

				if (d.errcode != null && (int)d.errcode != 0) {
					return new Failure<WeChatMiniProgramLoginResult>((int)d.errcode + ": " + d.errmsg);
				}
				return new Success<WeChatMiniProgramLoginResult> {
					Data = new() {
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

		public async Task<Result<WeChatUserPhoneResult>> GetUserPhoneAsync(WeChatGeneralAccessToken accessToken, string code) {
			var url = "https://api.weixin.qq.com/wxa/business/getuserphonenumber?access_token=" + accessToken.AccessToken;
			var parameters = new {
				code = code
			};

			try {
				var response = await WeChatService.HttpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(parameters)));
				var json = await response.Content.ReadAsStringAsync();
				var d = JsonConvert.DeserializeObject<dynamic>(json)!;

				var ok = d.errcode == null || (int)d.errcode == 0;
				if (!ok) {
					return new Failure<WeChatUserPhoneResult>((int)d.errcode + ": " + d.errmsg);
				}

				return new Success<WeChatUserPhoneResult> {
					Data = new() {
						PhoneNumber = d.phone_info.phoneNumber,
						PurePhoneNumber = d.phone_info.purePhoneNumber,
						CountryCode = d.phone_info.countryCode,
					}
				};
			}
			catch (Exception e) {
				return new Failure<WeChatUserPhoneResult>(e.Message);
			}
		}
	}
}
