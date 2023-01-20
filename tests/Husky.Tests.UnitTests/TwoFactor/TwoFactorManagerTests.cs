using System.Linq;
using Husky.Mail;
using Husky.Mail.Data;
using Husky.Principal;
using Husky.Sms.AliyunSms;
using Husky.TwoFactor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.TwoFactor.Tests
{
	[TestClass()]
	public class TwoFactorManagerTests
	{
		public TwoFactorManagerTests() {
			Crypto.SecretToken = Crypto.RandomString();

			var config = new ConfigurationManager();
			config.AddUserSecrets(GetType().Assembly);
			_aliyunSmsOptions = config.GetSection("AliyunSms").Get<AliyunSmsOptions>();
			_smtp = config.GetSection("Smtp").Get<MailSmtpProvider>();
		}

		private readonly AliyunSmsOptions _aliyunSmsOptions;
		private readonly MailSmtpProvider _smtp;
		private readonly string _givenCellphone = "17751283521";

		[TestMethod()]
		public void SendCodeThroughAliyunSmsTest() {
			if (string.IsNullOrEmpty(_aliyunSmsOptions.AccessKeySecret)) {
				return;
			}

			using var testDb = new DbContextOptionsBuilder<TwoFactorDbContext>().UseInMemoryDatabase("UnitTest").CreateDbContext();
			var principal = PrincipalUser.Personate(1, "TestUser", null);
			var smsSender = new AliyunSmsSender(_aliyunSmsOptions);
			var twoFactorManager = new TwoFactorManager(testDb, principal, smsSender, null);

			var sendTo = _givenCellphone;
			var sentResult = twoFactorManager.SendCodeThroughSmsAsync(sendTo).Result;
			var row = testDb.TwoFactorCodes.FirstOrDefault();

			Assert.IsTrue(sentResult.Ok);
			Assert.IsNotNull(row);
			Assert.AreEqual(sendTo, row.SentTo);
			Assert.IsFalse(row.IsUsed);

			var verifyResult = twoFactorManager.VerifyCodeAsync(sendTo, row.Code, true).Result;
			row = testDb.TwoFactorCodes.FirstOrDefault();
			Assert.IsTrue(verifyResult.Ok);
			Assert.AreEqual(sendTo, row.SentTo);
			Assert.IsTrue(row.IsUsed);

			testDb.Database.EnsureDeleted();
		}

		[TestMethod()]
		public void SendCodeThroughEmailTest() {
			using var mailDb = new DbContextOptionsBuilder<MailDbContext>().UseInMemoryDatabase("UnitTest").CreateDbContext();
			using var twoFactorDb = new DbContextOptionsBuilder<TwoFactorDbContext>().UseInMemoryDatabase("UnitTest").CreateDbContext();
			mailDb.Add(_smtp);
			mailDb.SaveChanges();

			var sendTo = _smtp.SenderMailAddress;
			var principal = PrincipalUser.Personate(1, "TestUser", null);
			var mailSender = new MailSender(mailDb);

			var twoFactorManager = new TwoFactorManager(twoFactorDb, principal, null, mailSender);

			var sentResult = twoFactorManager.SendCodeThroughEmailAsync(sendTo).Result;
			var row = twoFactorDb.TwoFactorCodes.FirstOrDefault();

			Assert.IsTrue(sentResult.Ok);
			Assert.IsNotNull(row);
			Assert.AreEqual(sendTo, row.SentTo);
			Assert.IsFalse(row.IsUsed);

			var verifyResult = twoFactorManager.VerifyCodeAsync(sendTo, row.Code, true).Result;
			row = twoFactorDb.TwoFactorCodes.FirstOrDefault();
			Assert.IsTrue(verifyResult.Ok);
			Assert.AreEqual(sendTo, row.SentTo);
			Assert.IsTrue(row.IsUsed);

			mailDb.Database.EnsureDeleted();
			twoFactorDb.Database.EnsureDeleted();
		}

		[TestMethod()]
		public void VerifyCodeTest() {
			using var testDb = new DbContextOptionsBuilder<TwoFactorDbContext>().UseInMemoryDatabase("UnitTest").CreateDbContext();
			var principal = PrincipalUser.Personate(1, "TestUser", null);

			var row = new TwoFactorCode {
				Code = "123456",
				UserId = principal.Id,
				SentTo = _givenCellphone
			};
			testDb.Add(row);
			testDb.SaveChanges();

			Result verifyResult = null;
			var twoFactorManager = new TwoFactorManager(testDb, principal, new AliyunSmsSender(new AliyunSmsOptions()), null);
			for (var i = 0; i < 12; i++) {
				verifyResult = twoFactorManager.VerifyCodeAsync(row.SentTo, "WRONG!", true).Result;
				Assert.IsFalse(verifyResult.Ok);
			}

			row = testDb.TwoFactorCodes.First();
			Assert.IsFalse(row.IsUsed);
			Assert.IsTrue(row.ErrorTimes > 10);

			//it still fails even the code is correct this time, because it has already failed for more than 10 times
			verifyResult = twoFactorManager.VerifyCodeAsync(row.SentTo, row.Code, true).Result;
			Assert.IsFalse(verifyResult.Ok);

			//reset then try a good one
			row.ErrorTimes = 0;
			testDb.SaveChanges();

			verifyResult = twoFactorManager.VerifyCodeAsync(row.SentTo, row.Code, true).Result;
			row = testDb.TwoFactorCodes.First();
			Assert.IsTrue(verifyResult.Ok);
			Assert.IsTrue(row.IsUsed);

			//if try one more time, it should fail because the code is used
			verifyResult = twoFactorManager.VerifyCodeAsync(row.SentTo, row.Code, true).Result;
			Assert.IsFalse(verifyResult.Ok);

			testDb.Database.EnsureDeleted();
		}
	}
}