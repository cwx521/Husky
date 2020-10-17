using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Husky.Mail.Data
{
	public partial class MailRecord
	{
		[Key]
		public int Id { get; set; }

		public Guid? SmtpId { get; set; }

		[StringLength(200), Required]
		public string Subject { get; set; } = null!;

		[Required]
		public string Body { get; set; } = null!;

		public bool IsHtml { get; set; }

		[StringLength(2000), Required]
		public string To { get; set; } = null!;

		[StringLength(2000)]
		public string? Cc { get; set; }

		[StringLength(500)]
		public string? Exception { get; set; }

		public bool IsSuccessful { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		// nav props

		public MailSmtpProvider? Smtp { get; set; }
		public List<MailRecordAttachment> Attachments { get; set; } = new List<MailRecordAttachment>();
	}
}
