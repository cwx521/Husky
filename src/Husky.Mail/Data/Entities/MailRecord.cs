using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Husky.Data.ModelBuilding.Annotations;

namespace Husky.Mail.Data
{
	public partial class MailRecord
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		public Guid? SmtpId { get; set; }

		[MaxLength(200)]
		public string Subject { get; set; }
		
		public string Body { get; set; }

		public bool IsHtml { get; set; }

		[MaxLength(2000)]
		public string To { get; set; }

		[MaxLength(2000)]
		public string Cc { get; set; }

		[MaxLength(500)]
		public string Exception { get; set; }

		public bool IsSuccessful { get; set; }

		[Index(IsClustered = true, IsUnique = false)]
		public DateTime CreateTime { get; set; } = DateTime.Now;


		public MailSmtpProvider Smtp { get; set; }
		public List<MailRecordAttachment> Attachments { get; set; }
	}
}
