using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Sms.AliyunSms.Tests
{
	[TestClass()]
	public class AliyunSmsSenderTests
	{
		//attention: fill the required values to run this test

		private readonly AliyunSmsSettings _settings = new AliyunSmsSettings {
			DefaultSignName = "星翼软件",
			DefaultTemplateCode = "SMS_170155854",
			AccessKeyId = "",
			AccessKeySecret = ""
		};

		[TestMethod()]
		public async Task SendAsyncTestAsync() {
			if ( string.IsNullOrEmpty(_settings.AccessKeySecret) ) {
				return;
			}

			var sendTo = "17751283521";
			var sender = new AliyunSmsSender(_settings);
			var arg = new SmsBody {
				Parameters = new Dictionary<string, string> {
					{ "code", new Random().Next(0, 1000000).ToString("D6") }
				}
			};
			await sender.SendAsync(arg, sendTo);
		}
	}
}