using System;
using System.ComponentModel.DataAnnotations;

namespace Husky.Mail.Data
{
	public partial class MailRecordAttachment
	{
		[Key]
		public int Id { get; set; }

		public int MailId { get; set; }

		[StringLength(100), Required]
		public string Name { get; set; } = null!;

		[Required]
		public byte[] ContentStream { get; set; } = null!;

		[StringLength(32), Required]
		public string ContentType { get; set; } = null!;

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		// nav props

		public MailRecord Mail { get; set; } = null!;
	}
}
