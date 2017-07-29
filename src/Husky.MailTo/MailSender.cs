using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Husky.MailTo;
using Husky.MailTo.Abstractions;
using Husky.MailTo.Data;
using Husky.Sugar;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using MimeKit.Text;

namespace Husky.Smtp
{
	public class MailSender : IMailSender
	{
		public MailSender(IServiceProvider serviceProvider) {
			_db = serviceProvider.GetRequiredService<MailDbContext>();
			_smtp = serviceProvider.GetService<ISmtpProvider>() ?? GetInternalSmtpProvider();
		}

		readonly MailDbContext _db;
		readonly ISmtpProvider _smtp;

		public MailDbContext DbContext => _db;

		public async Task Send(MailMessage mailMessage) => await Send(mailMessage, null);

		public async Task Send(MailMessage mailMessage, Action<MailSendCompletedEventArgs> onCompleted) {
			if ( mailMessage == null ) {
				throw new ArgumentNullException(nameof(mailMessage));
			}

			using ( var client = new SmtpClient() ) {
				var mailRecord = CreateMailRecord(mailMessage);

				_db.Add(mailRecord);
				await _db.SaveChangesAsync();

				try {
					client.Connect(_smtp.Host, _smtp.Port, false);
					client.Authenticate(_smtp.CredentialName, _smtp.Password);
					client.AuthenticationMechanisms.Remove("XOAUTH2");

					client.MessageSent += async (object sender, MessageSentEventArgs e) => {
						mailRecord.IsSuccessful = true;
						await _db.SaveChangesAsync();

						await Task.Run(() => {
							onCompleted?.Invoke(new MailSendCompletedEventArgs { MailMessage = mailMessage });
						});
					};
					await client.SendAsync(BuildMimeMessage(mailMessage));
				}
				catch ( Exception ex ) {
					mailRecord.Exception = ex.Message.Left(200);
					await _db.SaveChangesAsync();
				}
			}
		}

		private ISmtpProvider GetInternalSmtpProvider() {
			var haveCount = _db.MailSmtpProviders.Count(x => x.IsInUse);
			if ( haveCount == 0 ) {
				throw new ArgumentOutOfRangeException("（邮件发送模块）还没有配置任何SMTP服务。");
			}
			var skip = new Random().Next(0, haveCount);
			return _db.MailSmtpProviders.Skip(skip).First();
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

		private MimeMessage BuildMimeMessage(MailMessage mailMessage) {
			var mime = new MimeMessage();

			// Subject
			mime.Subject = mailMessage.Subject;
			// To
			mailMessage.To.ForEach(to => mime.To.Add(new MailboxAddress(to.Name, to.Address)));
			// Cc
			mailMessage.Cc.ForEach(cc => mime.Cc.Add(new MailboxAddress(cc.Name, cc.Address)));

			// Body
			var body = new TextPart(mailMessage.IsHtml ? TextFormat.Html : TextFormat.Text) {
				Text = mailMessage.Body
			};
			if ( mailMessage.Attachments?.Count == 0 ) {
				mime.Body = body;
			}
			// Or: Body + Attachments
			else {
				var multipart = new Multipart("mixed");
				mailMessage.Attachments.ForEach(a => {
					multipart.Add(new MimePart(a.ContentType) {
						ContentObject = new ContentObject(a.ContentStream),
						ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
						ContentTransferEncoding = ContentEncoding.Base64,
						FileName = a.Name
					});
				});
				multipart.Add(body);
				mime.Body = multipart;
			}
			return mime;
		}

		private byte[] ReadStream(Stream stream) {
			var length = stream.Length;
			var bytes = new byte[length];
			stream.Read(bytes, 0, (int)length);
			return bytes;
		}
	}
}