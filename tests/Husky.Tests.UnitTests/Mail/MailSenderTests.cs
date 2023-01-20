using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Husky.Mail.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Mail.Tests
{
	[TestClass()]
	public class MailSenderTests
	{
		public MailSenderTests() {
			Crypto.SecretToken = Crypto.RandomString();

			var config = new ConfigurationManager();
			config.AddUserSecrets(GetType().Assembly);
			_smtp = config.GetSection("Smtp").Get<MailSmtpProvider>();
		}

		private readonly MailSmtpProvider _smtp;
		private readonly string _givenEmailReceiver = "5607882@qq.com";

		[TestMethod()]
		public async Task SendAsyncTest() {
			using var db = new DbContextOptionsBuilder<MailDbContext>().UseInMemoryDatabase("UnitTest").CreateDbContext();
			db.Add(_smtp);
			db.SaveChanges();

			using var dummyAttachmentStream = new MemoryStream(Crypto.RandomBytes());
			var sender = new MailSender(db);
			var mail = new MailMessage {
				Subject = $"Husky.Mail Unit Test - {DateTime.Now:yyyy-M-d H:mm}",
				Body = "<div style='color:navy'>Greeting</div>",
				IsHtml = true,
				To = new List<MailAddress> {
					new MailAddress { Name = "Weixing", Address = _givenEmailReceiver }
				},
				Cc = new List<MailAddress> {
					new MailAddress { Name = "Weixing", Address = _smtp.SenderMailAddress }
				},
				Attachments = new List<MailAttachment> {
					new MailAttachment {
						Name = "DummyAttachment.zip",
						ContentType = "application/x-zip-compressed",
						ContentStream = dummyAttachmentStream
					}
				}
			};

			string strReadFromCallback = null;
			await sender.SendAsync(mail, arg => strReadFromCallback = arg.MailMessage.Body);

			var mailRecord = db.MailRecords
				.AsNoTracking()
				.Include(x => x.Smtp)
				.Include(x => x.Attachments)
				.OrderBy(x => x.Id)
				.LastOrDefault();

			Assert.IsNotNull(mailRecord);
			Assert.AreEqual(mailRecord.Subject, mail.Subject);
			Assert.AreEqual(mailRecord.Body, mail.Body);
			Assert.AreEqual(mailRecord.IsHtml, mail.IsHtml);
			Assert.AreEqual(mailRecord.Smtp.Id, _smtp.Id);
			Assert.AreEqual(mailRecord.To, string.Join(";", mail.To.Select(x => x.ToString())));
			Assert.AreEqual(mailRecord.Cc, string.Join(";", mail.Cc.Select(x => x.ToString())));
			Assert.AreEqual(mailRecord.Attachments.Count, mail.Attachments.Count);
			Assert.AreEqual(mailRecord.Attachments.First().Name, mail.Attachments.First().Name);
			Assert.AreEqual(mailRecord.Attachments.First().ContentBytes.Length, mail.Attachments.First().ContentStream.Length);

			Assert.AreEqual(mailRecord.Body, strReadFromCallback);
			Assert.AreEqual(mailRecord.IsSuccessful, true);
			Assert.IsTrue(string.IsNullOrEmpty(mailRecord.Exception));

			db.Database.EnsureDeleted();
		}
	}
}