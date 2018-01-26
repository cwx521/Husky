using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Husky.AspNetCore.Mail.Data;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using MimeKit.Text;

namespace Husky.AspNetCore.Mail
{
	public class MailSender : IMailSender
	{
		public MailSender(IServiceProvider serviceProvider) {
			_db = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<MailDbContext>();
			_givenSmtp = serviceProvider.GetService<ISmtpProvider>();
		}

		readonly MailDbContext _db;
		readonly ISmtpProvider _givenSmtp;

		public MailDbContext DbContext => _db;

		public async Task SendAsync(string subject, string content, params string[] recipients) {
			if ( recipients == null || recipients.Length == 0 ) {
				throw new ArgumentNullException(nameof(recipients));
			}
			await SendAsync(new MailMessage {
				Subject = subject,
				Body = content,
				IsHtml = true,
				To = recipients.Select(x => new MailAddress { Address = x }).ToList()
			});
		}

		public async Task SendAsync(MailMessage mailMessage) => await SendAsync(mailMessage, null);

		public async Task SendAsync(MailMessage mailMessage, Action<MailSentEventArgs> onCompleted) {
			if ( mailMessage == null ) {
				throw new ArgumentNullException(nameof(mailMessage));
			}

			var smtp = _givenSmtp ?? GetInternalSmtpProvider();
			var mailRecord = CreateMailRecord(mailMessage);

			if ( smtp is MailSmtpProvider internalSmtp ) {
				mailRecord.SmtpId = internalSmtp.Id;
			}

			_db.Add(mailRecord);
			await _db.SaveChangesAsync();

			using ( var client = new SmtpClient() ) {
				client.MessageSent += async (object sender, MessageSentEventArgs e) => {
					mailRecord.IsSuccessful = true;
					await _db.SaveChangesAsync();
					await Task.Run(() => {
						onCompleted?.Invoke(new MailSentEventArgs { MailMessage = mailMessage });
					});
				};
				try {
					await client.ConnectAsync(smtp.Host, smtp.Port, smtp.Ssl);

					if ( !string.IsNullOrEmpty(smtp.CredentialName) ) {
						await client.AuthenticateAsync(smtp.CredentialName, smtp.Password);
					}
					await client.SendAsync(BuildMimeMessage(smtp, mailMessage));
				}
				catch ( Exception ex ) {
					mailRecord.Exception = ex.Message.Left(200);
					await _db.SaveChangesAsync();
				}
			}
		}

		static int _increment = 0;
		private MailSmtpProvider GetInternalSmtpProvider() {
			var haveCount = _db.MailSmtpProviders.Count(x => x.IsInUse);
			if ( haveCount == 0 ) {
				throw new Exception("SMTP account is not configured yet.");
			}
			var skip = _increment++ % haveCount;
			return _db.MailSmtpProviders.Where(x => x.IsInUse).AsNoTracking().Skip(skip).First();
		}

		private MailRecord CreateMailRecord(MailMessage mailMessage) {
			return new MailRecord {
				Subject = mailMessage.Subject,
				Body = mailMessage.Body,
				IsHtml = mailMessage.IsHtml,

				To = string.Join(";", mailMessage.To.Select(x => x.ToString())),
				Cc = string.Join(";", mailMessage.Cc.Select(x => x.ToString())),

				Attachments = mailMessage.Attachments.Select(a => new MailRecordAttachment {
					Name = a.Name,
					ContentStream = ReadStream(a.ContentStream),
					ContentType = a.ContentType
				}).ToList()
			};
		}

		private MimeMessage BuildMimeMessage(ISmtpProvider smtp, MailMessage mailMessage) {
			var mail = new MimeMessage();

			// Subject
			mail.Subject = mailMessage.Subject;
			// From
			mail.From.Add(new MailboxAddress(smtp.SenderDisplayName, smtp.SenderMailAddress ?? smtp.CredentialName));
			// To
			mailMessage.To.ForEach(to => mail.To.Add(new MailboxAddress(to.Name, to.Address)));
			// Cc
			mailMessage.Cc.ForEach(cc => mail.Cc.Add(new MailboxAddress(cc.Name, cc.Address)));

			// Body
			var body = new TextPart(mailMessage.IsHtml ? TextFormat.Html : TextFormat.Text) {
				Text = mailMessage.Body
			};
			if ( mailMessage.Attachments?.Count == 0 ) {
				mail.Body = body;
			}
			// Or: Body + Attachments
			else {
				var multipart = new Multipart("mixed");
				mailMessage.Attachments.ForEach(a => {
					multipart.Add(new MimePart(a.ContentType) {
						Content = new MimeContent(a.ContentStream),
						ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
						ContentTransferEncoding = ContentEncoding.Base64,
						FileName = a.Name
					});
				});
				multipart.Add(body);
				mail.Body = multipart;
			}
			return mail;
		}

		private byte[] ReadStream(Stream stream) {
			var length = stream.Length;
			var bytes = new byte[length];
			stream.Read(bytes, 0, (int)length);
			return bytes;
		}
	}
}