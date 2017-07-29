using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Husky.Data.ModelBuilding.Annotations;

namespace Husky.MailTo.Data
{
	public partial class MailRecord
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		[MaxLength(200)]
		public string Subject { get; set; }

		[MaxLength(4000)]
		public string Body { get; set; }

		public bool IsHtml { get; set; }

		[MaxLength(2000)]
		public string To { get; set; }

		[MaxLength(2000)]
		public string Cc { get; set; }

		public bool IsSuccessful { get; set; }

		[Index(IsUnique = false, IsClustered = true)]
		public DateTime CreateTime { get; set; } = DateTime.Now;


		public List<MailRecordAttachment> Attachments { get; set; }
	}
}
