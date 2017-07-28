//using System;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using Husky.Smtp.Abstractions;

//namespace Husky.Smtp
//{
//	public class MailSender // : IMailSender
//	{
//		public MailSender(ISmtpProvider smtpProvider) {
//			if ( smtpProvider == null ) {
//				throw new ArgumentNullException(nameof(smtpProvider));
//			}
//			_smtp = smtpProvider;
//		}

//		private readonly ISmtpProvider _smtp;

//		public async Task SendAsync(MailMessage mail, Action<AsyncCompletedEventArgs> onCompleted = null) {
//			if ( mail == null ) {
//				throw new ArgumentNullException(nameof(mail));
//			}
//			using ( var svc = new SmtpClient() ) {
//				svc.Host = _smtp.Host;
//				svc.Port = _smtp.Port;
//				svc.Credentials = new NetworkCredential(_smtp.CredentialName, _smtp.Password);
//				svc.SendCompleted += async (sender, e) => {
//					await SmtpMailRecord.Add(mail, e);
//					onCompleted?.Invoke(e);
//				};
//				await svc.SendMailAsync(mail);
//			}
//		}

//		public async Task SendAsync(string subject, string content, string[] sendTo, string[] cc, Attachment[] attachments, Action<AsyncCompletedEventArgs> onCompleted = null) {
//			if ( string.IsNullOrEmpty(subject) ) {
//				subject = "(No subject)";
//			}
//			if ( string.IsNullOrEmpty(content) ) {
//				throw new ArgumentNullException(nameof(content), $"Argument {nameof(content)} can not be null or empty.");
//			}
//			if ( sendTo == null || sendTo.Length == 0 ) {
//				throw new ArgumentNullException(nameof(content), $"Argument {nameof(sendTo)} is null or empty. Must specify one or more recipients.");
//			}
//			using ( var mail = new MailMessage() ) {
//				mail.Subject = subject;
//				mail.Body = content;
//				mail.BodyEncoding = Encoding.UTF8;
//				mail.From = new MailAddress(_smtp.DisplayMailAddress, _smtp.DisplayName);
//				for ( var i = 0; i < sendTo.Length; mail.To.Add(sendTo[i++]) ) ;
//				for ( var i = 0; i < cc?.Length; mail.CC.Add(cc[i++]) ) ;
//				for ( var i = 0; i < attachments?.Length; mail.Attachments.Add(attachments[i++]) ) ;
//				await SendAsync(mail, onCompleted);
//			}
//		}

//		public async Task SendAsync(string subject, string content, string[] sendTo, string[] cc, Action<AsyncCompletedEventArgs> onCompleted = null) {
//			await SendAsync(subject, content, sendTo, cc, null, onCompleted);
//		}

//		public async Task SendAsync(string subject, string content, string[] sendTo, Action<AsyncCompletedEventArgs> onCompleted = null) {
//			await SendAsync(subject, content, sendTo, null, null, onCompleted);
//		}
//	}
//}