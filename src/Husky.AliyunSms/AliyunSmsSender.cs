using System;
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

		public async Task<Result> SendAsync(ISmsBody sms, params string[] toMobileNumbers) {
			if ( toMobileNumbers == null || toMobileNumbers.Length == 0 ) {
				throw new ArgumentNullException(nameof(toMobileNumbers));
			}

			var request = new SendSmsRequest {
				PhoneNumbers = string.Join(",", toMobileNumbers),
				SignName = sms.SignName ?? _settings.DefaultSignName,
				TemplateCode = sms.TemplateAlias ?? _settings.DefaultTemplateCode,
				TemplateParam = JsonConvert.SerializeObject(sms.Parameters)
			};
			var profile = DefaultProfile.GetProfile(_settings.EndPointRegion, _settings.AccessKeyId, _settings.AccessKeySecret);
			var client = new DefaultAcsClient(profile);

			return await Task.Run<Result>(() => {
				try {
					client.DoAction(request);
					return new Success();
				}
				catch ( Exception e ) {
					return new Failure(e.Message);
				}
			});
		}
	}
}
