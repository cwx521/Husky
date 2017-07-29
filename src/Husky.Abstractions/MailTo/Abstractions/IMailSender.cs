using System;
using System.Threading.Tasks;

namespace Husky.MailTo.Abstractions
{
	public interface IMailSender
    {
		Task Send(MailMessage mailMessage);
		Task Send(MailMessage mailMessage, Action<MailSendCompletedEventArgs> onCompleted);
	}
}
