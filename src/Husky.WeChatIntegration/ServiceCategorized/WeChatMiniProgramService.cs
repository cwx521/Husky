using System;
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
	}
}
