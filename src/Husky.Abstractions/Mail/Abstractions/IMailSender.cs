using System;
using System.Threading.Tasks;

namespace Husky.Mail.Abstractions
{
	public interface IMailSender
	{
		Task Send(MailMessage mailMessage);
		Task Send(MailMessage mailMessage, Action<MailSentEventArgs> onCompleted);
		Task Send(string recipient, string subject, string content);
	}
}
