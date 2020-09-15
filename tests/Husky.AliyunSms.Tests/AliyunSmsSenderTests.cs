using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.AliyunSms.Tests
{
	[TestClass()]
	public class AliyunSmsSenderTests
	{
		private readonly AliyunSmsSettings _settings = new AliyunSmsSettings {
			DefaultSignName = "星翼软件",
			DefaultTemplateCode = "SMS_170155854",
			AccessKeyId = "LTAI4FqwMTMob4TH9MP5dfTK",
			AccessKeySecret = ""
		};

		[TestMethod()]
		public void SendAsyncTest() {
			if ( string.IsNullOrEmpty(_settings.AccessKeySecret) ) {
				return;
			}

			var sendTo = "17751283521";
			var sender = new AliyunSmsSender(_settings);
			var arg = new AliyunSmsArgument {
				Parameters = new Dictionary<string, string> {
					{ "code", new Random().Next(0, 1000000).ToString("D6") }
				}
			};
			sender.SendAsync(arg, sendTo).Wait();
		}
	}
}