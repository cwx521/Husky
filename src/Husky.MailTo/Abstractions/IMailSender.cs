//using System;
//using System.ComponentModel;
//using System.Net.Mail;
//using System.Threading.Tasks;

//namespace Husky.MailTo.Abstractions
//{
//	public interface IMailSender
//	{
//		Task SendAsync(MailMessage mailMessage, Action<AsyncCompletedEventArgs> onCompleted);
//		Task SendAsync(string subject, string content, string[] sendTo, Action<AsyncCompletedEventArgs> onCompleted);
//		Task SendAsync(string subject, string content, string[] sendTo, string[] cc, Action<AsyncCompletedEventArgs> onCompleted);
//	}
//}