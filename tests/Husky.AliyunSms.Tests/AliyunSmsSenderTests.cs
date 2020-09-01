using Microsoft.VisualStudio.TestTools.UnitTesting;
using Husky.AliyunSms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Husky.AliyunSms.Tests
{
	[TestClass()]
	public class AliyunSmsSenderTests
	{
		[TestMethod()]
		public void SendAsyncTest() {
			var settings = new AliyunSmsSettings {
				DefaultSignName = "",
				DefaultTemplateCode = "",
				AccessKeyId = "",
				AccessKeySecret = ""
			};
			var sender = new AliyunSmsSender(settings);
		}
	}
}