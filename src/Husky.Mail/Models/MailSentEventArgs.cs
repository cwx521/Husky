namespace Husky.Mail
{
	public sealed class MailSentEventArgs
	{
		public MailMessage MailMessage { get; set; } = null!;
	}
}
