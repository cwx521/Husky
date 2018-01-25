using System;
using System.ComponentModel.DataAnnotations;

namespace Husky.AspNetCore.Mail.Data
{
	public partial class MailRecordAttachment
	{
		[Key]
		public int Id { get; set; }

		public int MailId { get; set; }

		[MaxLength(100)]
		public string Name { get; set; }

		public byte[] ContentStream { get; set; }

		[MaxLength(32)]
		public string ContentType { get; set; }

		public DateTime CreatedTime { get; set; }


		public virtual MailRecord Mail { get; set; }
	}
}
