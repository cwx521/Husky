using System.Collections.Generic;

namespace Husky.Mail
{
	public sealed class MailMessage
	{
		public string Subject { get; set; } = null!;
		public string Body { get; set; } = null!;
		public bool IsHtml { get; set; } = true;
		public List<MailAddress> To { get; set; } = new List<MailAddress>();
		public List<MailAddress> Cc { get; set; } = new List<MailAddress>();
		public List<MailAttachment> Attachments { get; set; } = new List<MailAttachment>();
	}
}
