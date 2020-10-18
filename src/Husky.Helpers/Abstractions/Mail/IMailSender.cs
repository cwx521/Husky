using System;
using System.Threading.Tasks;

namespace Husky.Mail
{
	public interface IMailSender
	{
		Task SendAsync(MailMessage mailMessage, Action<MailSentEventArgs>? onCompleted);
		Task SendAsync(string subject, string content, params string[] recipients);
	}
}
