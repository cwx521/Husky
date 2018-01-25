using System.Collections.Generic;

namespace Husky.AspNetCore.Mail
{
	public sealed class MailMessage
	{
		public string Subject { get; set; }
		public string Body { get; set; }
		public bool IsHtml { get; set; } = true;
		public List<MailAddress> To { get; set; } = new List<MailAddress>();
		public List<MailAddress> Cc { get; set; } = new List<MailAddress>();
		public List<MailAttachment> Attachments { get; set; } = new List<MailAttachment>();
	}
}
