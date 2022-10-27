namespace Husky.Mail
{
	public sealed class MailSentEventArgs
	{
		public int MailRecordId { get; set; }
		public MailMessage MailMessage { get; set; } = null!;
	}
}
