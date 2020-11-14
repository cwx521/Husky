namespace Husky.Sms.AliyunSms
{
	public class AliyunSmsOptions
	{
		public string AccessKeyId { get; set; } = null!;
		public string AccessKeySecret { get; set; } = null!;

		public string? DefaultTemplateCode { get; set; }
		public string? DefaultSignName { get; set; }

		public string Product { get; set; } = "Dysmsapi";
		public string EndPointRegion { get; set; } = "cn-hangzhou";
		public string Domain { get; set; } = "dysmsapi.aliyuncs.com";
	}
}
