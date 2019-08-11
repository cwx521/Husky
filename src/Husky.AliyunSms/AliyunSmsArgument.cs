using System.Collections.Generic;

namespace Husky.AliyunSms
{
	public class AliyunSmsArgument
	{
		public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
		public string TemplateCode { get; set; }
		public string SignName { get; set; }
	}
}
