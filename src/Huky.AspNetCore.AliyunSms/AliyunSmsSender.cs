using System.Threading.Tasks;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Aliyun.Net.SDK.Core;
using Aliyun.Net.SDK.Core.Profile;
using Newtonsoft.Json;

namespace Husky.AspNetCore.AliyunSms
{
	public class AliyunSmsSender
	{
		public async Task SendAsync(AliyunSmsConfig aliyunSmsModel, AliyunSmsArgument argument, params string[] phoneNumbers) {
			if ( phoneNumbers == null || phoneNumbers.Length == 0 ) {
				return;
			}

			var request = new SendSmsRequest {
				PhoneNumbers = string.Join(",", phoneNumbers),
				SignName = aliyunSmsModel.SignName,
				TemplateCode = aliyunSmsModel.TemplateCode,
				TemplateParam = JsonConvert.SerializeObject(argument)
			};

			var endPointRegion = "cn-hangzhou";
			DefaultProfile.AddEndpoint(endPointRegion, endPointRegion, "Dysmsapi", "dysmsapi.aliyuncs.com");

			await Task.Run(() => {
				var profile = DefaultProfile.GetProfile(endPointRegion, aliyunSmsModel.AccessKeyId, aliyunSmsModel.AccessKeySecret);
				new DefaultAcsClient(profile).GetAcsResponse(request);
			});
		}
	}
}
