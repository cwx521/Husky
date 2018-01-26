namespace Husky.AliyunSms
{
	public class AliyunSmsConfig
	{
		public const string SectionName = "AliyunSms";

		public string AccessKeyId { get; set; }
		public string AccessKeySecret { get; set; }
		public string SignName { get; set; }
		public string TemplateCode { get; set; }
	}
}
