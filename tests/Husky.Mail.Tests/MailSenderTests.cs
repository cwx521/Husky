using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Husky.Mail.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Mail.Tests
{
	[TestClass()]
	public class MailSenderTests
	{
		[TestMethod()]
		public async Task SendAsyncTest() {
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

			using var testDb = new DbContextOptionsBuilder<MailDbContext>().UseInMemoryDatabase("UnitTest").CreateDbContext();

			testDb.Add(smtp);
			testDb.SaveChanges();

			var sender = new MailSender(testDb);
			var mail = new MailMessage {
				Subject = "Husky.Mail Unit Test",
				Body = "<div style='color:blue'>Greeting</div>",
				IsHtml = true,
				To = new List<MailAddress> {
					new MailAddress { Name = "Weixing", Address = "chenwx521@hotmail.com" }
				},
				Cc = new List<MailAddress> {
					new MailAddress { Name = "Weixing", Address = "5607882@qq.com" }
				},
				Attachments = new List<MailAttachment> {
					new MailAttachment {
						Name = "DummyAttachment.zip",
						ContentType = "application/x-zip-compressed",
						ContentStream = new MemoryStream(Crypto.RandomBytes())
					}
				}
			};

			var i = 0;
			string str = null;
			await sender.SendAsync(mail, (arg) => { i++; str = arg.MailMessage.Body; });

			var mailRecord = testDb.MailRecords
				.AsNoTracking()
				.Include(x => x.Smtp)
				.Include(x => x.Attachments)
				.OrderBy(x => x.Id)
				.LastOrDefault();

			Assert.IsNotNull(mailRecord);
			Assert.AreEqual(mailRecord.Subject, mail.Subject);
			Assert.AreEqual(mailRecord.Body, mail.Body);
			Assert.AreEqual(mailRecord.IsHtml, mail.IsHtml);
			Assert.AreEqual(mailRecord.Smtp.Id, smtp.Id);
			Assert.AreEqual(mailRecord.To, string.Join(";", mail.To.Select(x => x.ToString())));
			Assert.AreEqual(mailRecord.Cc, string.Join(";", mail.Cc.Select(x => x.ToString())));
			Assert.AreEqual(mailRecord.Attachments.Count, mail.Attachments.Count);
			Assert.AreEqual(mailRecord.Attachments.First().Name, mail.Attachments.First().Name);

			Assert.AreEqual(i, 1);
			Assert.AreEqual(str, mail.Body);
			Assert.AreEqual(mailRecord.IsSuccessful, true);
			Assert.IsTrue(string.IsNullOrEmpty(mailRecord.Exception));

			testDb.Database.EnsureDeleted();
		}
	}
}