using System.Threading.Tasks;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Aliyun.Net.SDK.Core;
using Aliyun.Net.SDK.Core.Profile;
using Newtonsoft.Json;

namespace Husky.Sms.AliyunSms
{
	public class AliyunSmsSender : ISmsSender
	{
		public AliyunSmsSender(AliyunSmsSettings settings) {
			_settings = settings;
			DefaultProfile.AddEndpoint(_settings.EndPointRegion, _settings.EndPointRegion, _settings.Product, _settings.Domain);
		}

		private readonly AliyunSmsSettings _settings;

		public async Task SendAsync(ISmsBody sms, params string[] toMobileNumbers) {
			if ( toMobileNumbers == null || toMobileNumbers.Length == 0 ) {
				return;
			}

			var request = new SendSmsRequest {
				PhoneNumbers = string.Join(",", toMobileNumbers),
				SignName = sms.SignName ?? _settings.DefaultSignName,
				TemplateCode = sms.TemplateCode ?? _settings.DefaultTemplateCode,
				TemplateParam = JsonConvert.SerializeObject(sms.Parameters)
			};

			await Task.Run(() => {
				try {
					var profile = DefaultProfile.GetProfile(_settings.EndPointRegion, _settings.AccessKeyId, _settings.AccessKeySecret);
					var client = new DefaultAcsClient(profile);
					client.GetAcsResponse(request);
				}
				catch { }
			});
		}
	}
}
