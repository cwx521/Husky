using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Husky.Mail.Data;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;

namespace Husky.Mail
{
	public class MailSender : IMailSender
	{
		public MailSender(IMailDbContext mailDb) {
			_mailDb = mailDb;
		}

		public MailSender(IMailDbContext mailDb, ISmtpProvider givenSmtp) {
			_mailDb = mailDb;
			_smtp = givenSmtp;
		}

		private readonly IMailDbContext _mailDb;
		private readonly ISmtpProvider? _smtp;

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

		public async Task SendAsync(MailMessage mailMessage, Action<MailSentEventArgs>? onCompleted = null) {
			if ( mailMessage == null ) {
				throw new ArgumentNullException(nameof(mailMessage));
			}

			var smtp = _smtp ?? await GetPreconfiguredSmtpProviderAsync(_mailDb);
			var mailRecord = await BuildMailRecordAsync(mailMessage);

			if ( smtp is MailSmtpProvider internalSmtp ) {
				mailRecord.SmtpId = internalSmtp.Id;
			}

			_mailDb.MailRecords.Add(mailRecord);
			await _mailDb.Normalize().SaveChangesAsync();

			using var client = new SmtpClient();

			client.MessageSent += async (object? sender, MessageSentEventArgs? e) => {

				mailRecord.IsSuccessful = true;
				await _mailDb.Normalize().SaveChangesAsync();

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
				mailRecord.Exception = ex.Message.Left(500);
				await _mailDb.Normalize().SaveChangesAsync();
			}
		}

		private static int _increment = 0;

		private static async Task<MailSmtpProvider> GetPreconfiguredSmtpProviderAsync(IMailDbContext mailDb) {
			var available = mailDb.MailSmtpProviders.Count(x => x.IsInUse);
			if ( available == 0 ) {
				throw new Exception("SMTP is not configured yet.");
			}
			var skip = _increment++ % available;
			return await mailDb.MailSmtpProviders.Where(x => x.IsInUse).AsNoTracking().Skip(skip).FirstAsync();
		}

		private static async Task<MailRecord> BuildMailRecordAsync(MailMessage mail) {
			var mailRecord = new MailRecord {
				Subject = mail.Subject,
				Body = mail.Body,
				IsHtml = mail.IsHtml,
				To = string.Join(";", mail.To.Select(x => x.ToString())),
				Cc = string.Join(";", mail.Cc.Select(x => x.ToString())),
			};
			foreach ( var item in mail.Attachments ) {
				mailRecord.Attachments.Add(new MailRecordAttachment {
					Name = item.Name,
					ContentStream = await ReadStreamAsync(item.ContentStream),
					ContentType = item.ContentType
				});
			}
			return mailRecord;
		}

		private static MimeMessage BuildMimeMessage(ISmtpProvider smtp, MailMessage mail) {
			var mimeMessage = new MimeMessage();

			mimeMessage.From.Add(new MailboxAddress(smtp.SenderDisplayName, smtp.SenderMailAddress ?? smtp.CredentialName));
			mimeMessage.To.AddRange(mail.To.Select(to => new MailboxAddress(to.Name, to.Address)));
			mimeMessage.Cc.AddRange(mail.Cc.Select(cc => new MailboxAddress(cc.Name, cc.Address)));
			mimeMessage.Subject = mail.Subject;

			var body = new TextPart(mail.IsHtml ? TextFormat.Html : TextFormat.Text) {
				Text = mail.Body
			};

			if ( mail.Attachments.Count == 0 ) {
				mimeMessage.Body = body;
			}
			else {
				var multipart = new Multipart("mixed");
				mail.Attachments.AsParallel().ForAll(x => {
					multipart.Add(new MimePart(x.ContentType) {
						FileName = x.Name,
						Content = new MimeContent(x.ContentStream),
						ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
						ContentTransferEncoding = ContentEncoding.Base64
					});
				});
				multipart.Add(body);
				mimeMessage.Body = multipart;
			}

			return mimeMessage;
		}

		private static async Task<byte[]> ReadStreamAsync(Stream stream) {
			var length = stream.Length;
			var bytes = new byte[length];
			await stream.ReadAsync(bytes);
			return bytes;
		}
	}
}