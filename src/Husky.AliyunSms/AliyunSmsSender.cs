using System.Threading.Tasks;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Aliyun.Net.SDK.Core;
using Aliyun.Net.SDK.Core.Profile;
using Newtonsoft.Json;

namespace Husky.AliyunSms
{
	public class AliyunSmsSender
	{
		public AliyunSmsSender(AliyunSmsSettings settings) {
			_settings = settings;
		}

		private readonly AliyunSmsSettings _settings;

		public async Task SendAsync(AliyunSmsArgument argument, params string[] mobileNumbers) {
			if ( mobileNumbers == null || mobileNumbers.Length == 0 ) {
				return;
			}

			var request = new SendSmsRequest {
				PhoneNumbers = string.Join(",", mobileNumbers),
				SignName = argument.SignName ?? _settings.DefaultSignName,
				TemplateCode = argument.TemplateCode ?? _settings.DefaultTemplateCode,
				TemplateParam = JsonConvert.SerializeObject(argument.Parameters)
			};

			var endPointRegion = "cn-hangzhou";
			DefaultProfile.AddEndpoint(endPointRegion, endPointRegion, "Dysmsapi", "dysmsapi.aliyuncs.com");

			await Task.Run(() => {
				try {
					var profile = DefaultProfile.GetProfile(endPointRegion, _settings.AccessKeyId, _settings.AccessKeySecret);
					var client = new DefaultAcsClient(profile);
					client.GetAcsResponse(request);
				}
				catch { }
			});
		}
	}
}
