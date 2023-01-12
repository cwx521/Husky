using System;
using System.Linq;
using System.Threading.Tasks;
using Husky.Mail;
using Husky.Mail.Data;
using Husky.Principal;
using Husky.Sms.AliyunSms;
using Husky.TwoFactor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.TwoFactor.Tests
{
	[TestClass()]
	public class TwoFactorManagerTests
	{
		//attention: fill the required values to run this test

		[TestMethod()]
		public void SendCodeThroughAliyunSmsTest() {
			var settings = new AliyunSmsOptions {
				DefaultSignName = "星翼软件",
				DefaultTemplateCode = "SMS_170155854",
				AccessKeyId = "",
				AccessKeySecret = ""
			};

			if (string.IsNullOrEmpty(settings.AccessKeySecret)) {
				return;
			}

			using var testDb = new DbContextOptionsBuilder<TwoFactorDbContext>().UseInMemoryDatabase("UnitTest").CreateDbContext();
			var principal = PrincipalUser.Personate(1, "TestUser", null);
			var smsSender = new AliyunSmsSender(settings);
			var twoFactorManager = new TwoFactorManager(testDb, principal, smsSender, null);

			var sendTo = "17751283521";
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
			Crypto.SecretToken = Crypto.RandomString();

			var senderAddress = "chenwx@xingyisoftware.com";
			var smtp = new MailSmtpProvider {
				Id = Guid.NewGuid(),
				Host = "smtp.exmail.qq.com",
				Port = 465,
				Ssl = true,
				SenderDisplayName = "Weixing Chen",
				SenderMailAddress = senderAddress,
				CredentialName = senderAddress,
				Password = "",
				IsInUse = true
			};

			//Config CredentialName & Password before running this test

			if (string.IsNullOrEmpty(smtp.CredentialName) || string.IsNullOrEmpty(smtp.Password)) {
				return;
			}

			using var mailDb = new DbContextOptionsBuilder<MailDbContext>().UseInMemoryDatabase("UnitTest").CreateDbContext();
			using var twoFactorDb = new DbContextOptionsBuilder<TwoFactorDbContext>().UseInMemoryDatabase("UnitTest").CreateDbContext();
			mailDb.Add(smtp);
			mailDb.SaveChanges();

			var sendTo = senderAddress;
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
				SentTo = "17751283521"
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