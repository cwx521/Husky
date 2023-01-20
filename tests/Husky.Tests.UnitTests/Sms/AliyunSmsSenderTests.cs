using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Sms.AliyunSms.Tests
{
	[TestClass()]
	public class AliyunSmsSenderTests
	{
		public AliyunSmsSenderTests() {
			var config = new ConfigurationManager();
			config.AddUserSecrets(GetType().Assembly);
			_aliyunSmsOptions = config.GetSection("AliyunSms").Get<AliyunSmsOptions>();
		}

		private readonly AliyunSmsOptions _aliyunSmsOptions;
		private readonly string _givenCellphone = "17751283521";

		private readonly bool _skipThisTest = true;

		[TestMethod()]
		public async Task SendAsyncTestAsync() {
			if (_skipThisTest) {
				return;
			}

			var sendTo = _givenCellphone;
			var sender = new AliyunSmsSender(_aliyunSmsOptions);
			var arg = new SmsBody {
				Parameters = new Dictionary<string, string> {
					{ "code", new Random().Next(0, 1000000).ToString("D6") }
				}
			};
			var result = await sender.SendAsync(arg, sendTo);
			Assert.IsTrue(result.Ok);
		}
	}
}