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
		public AliyunSmsSender(AliyunSmsOptions options) {
			_options = options;
			DefaultProfile.AddEndpoint(_options.EndPointRegion, _options.EndPointRegion, _options.Product, _options.Domain);
		}

		private readonly AliyunSmsOptions _options;

		public async Task<Result> SendAsync(ISmsBody sms, params string[] toMobileNumbers) {
			if ( toMobileNumbers == null || toMobileNumbers.Length == 0 ) {
				throw new ArgumentNullException(nameof(toMobileNumbers));
			}

			var request = new SendSmsRequest {
				PhoneNumbers = string.Join(",", toMobileNumbers),
				SignName = sms.SignName ?? _options.DefaultSignName,
				TemplateCode = sms.TemplateAlias ?? _options.DefaultTemplateCode,
				TemplateParam = JsonConvert.SerializeObject(sms.Parameters)
			};
			var profile = DefaultProfile.GetProfile(_options.EndPointRegion, _options.AccessKeyId, _options.AccessKeySecret);
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
