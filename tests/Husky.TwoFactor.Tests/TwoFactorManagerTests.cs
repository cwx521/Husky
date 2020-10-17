using System;
using System.Linq;
using Husky.AliyunSms;
using Husky.Mail;
using Husky.Mail.Data;
using Husky.Principal;
using Husky.TwoFactor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.TwoFactor.Tests
{
	[TestClass()]
	public class TwoFactorManagerTests
	{
		[TestMethod()]
		public void RequestCodeThroughAliyunSmsTest() {
			var settings = new AliyunSmsSettings {
				DefaultSignName = "星翼软件",
				DefaultTemplateCode = "SMS_170155854",
				AccessKeyId = "LTAI4FqwMTMob4TH9MP5dfTK",
				AccessKeySecret = ""
			};

			if ( string.IsNullOrEmpty(settings.AccessKeySecret) ) {
				return;
			}

			var dbName = $"UnitTest_{nameof(TwoFactorManagerTests)}_{nameof(RequestCodeThroughAliyunSmsTest)}";
			var dbBuilder = new DbContextOptionsBuilder<TwoFactorDbContext>();
			dbBuilder.UseSqlServer($"Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog={dbName}; Integrated Security=True");

			using var db = dbBuilder.CreateDbContext();
			db.Database.EnsureDeleted();
			db.Database.Migrate();

			var sendTo = "17751283521";
			var principal = PrincipalUser.Personate(1, "TestUser", null);
			var smsSender = new AliyunSmsSender(settings);

			var twoFactorManager = new TwoFactorManager(principal, db, smsSender, null);

			var sentResult = twoFactorManager.RequestCodeThroughAliyunSms(sendTo).Result;
			var row = db.TwoFactorCodes.FirstOrDefault();

			Assert.IsTrue(sentResult.Ok);
			Assert.IsNotNull(row);
			Assert.AreEqual(sendTo, row.SentTo);
			Assert.IsFalse(row.IsUsed);

			var verifyResult = twoFactorManager.VerifyCode(sendTo, row.Code, true).Result;
			row = db.TwoFactorCodes.FirstOrDefault();
			Assert.IsTrue(verifyResult.Ok);
			Assert.AreEqual(sendTo, row.SentTo);
			Assert.IsTrue(row.IsUsed);

			db.Database.EnsureDeleted();
		}

		[TestMethod()]
		public void RequestCodeThroughEmailTest() {
			Crypto.PermanentToken = Crypto.RandomString();

			var smtp = new MailSmtpProvider {
				Id = Guid.NewGuid(),
				Host = "smtp.live.com",
				Port = 25,
				Ssl = false,
				SenderDisplayName = "Weixing Chen",
				SenderMailAddress = "chenwx521@hotmail.com",
				CredentialName = "chenwx521@hotmail.com",
				Password = "",
				IsInUse = true
			};

			//Config CredentialName&Password before running this test

			if ( string.IsNullOrEmpty(smtp.CredentialName) || string.IsNullOrEmpty(smtp.Password) ) {
				return;
			}

			var dbName = $"UnitTest_{nameof(TwoFactorManagerTests)}_{nameof(RequestCodeThroughEmailTest)}";
			var dbBuilder = new DbContextOptionsBuilder<MailDbContext>();
			dbBuilder.UseSqlServer($"Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog={dbName}; Integrated Security=True");

			using var mailDb = new MailDbContext(dbBuilder.Options);
			mailDb.Database.EnsureDeleted();
			mailDb.Database.Migrate();
			mailDb.Add(smtp);
			mailDb.SaveChanges();

			var dbBuilder2 = new DbContextOptionsBuilder<TwoFactorDbContext>();
			dbBuilder2.UseSqlServer($"Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog={dbName}; Integrated Security=True");

			using var twoFactorDb = dbBuilder2.CreateDbContext();
			twoFactorDb.Database.Migrate();

			var sendTo = "chenwx521@hotmail.com";
			var principal = PrincipalUser.Personate(1, "TestUser", null);
			var mailSender = new MailSender(mailDb);

			var twoFactorManager = new TwoFactorManager(principal, twoFactorDb, null, mailSender);

			var sentResult = twoFactorManager.RequestCodeThroughEmail(sendTo).Result;
			var row = twoFactorDb.TwoFactorCodes.FirstOrDefault();

			Assert.IsTrue(sentResult.Ok);
			Assert.IsNotNull(row);
			Assert.AreEqual(sendTo, row.SentTo);
			Assert.IsFalse(row.IsUsed);

			var verifyResult = twoFactorManager.VerifyCode(sendTo, row.Code, true).Result;
			row = twoFactorDb.TwoFactorCodes.FirstOrDefault();
			Assert.IsTrue(verifyResult.Ok);
			Assert.AreEqual(sendTo, row.SentTo);
			Assert.IsTrue(row.IsUsed);

			mailDb.Database.EnsureDeleted();
		}
	}
}