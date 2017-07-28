using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Husky.Data.ModelBuilding.Annotations;

namespace Husky.MailTo.Data
{
	public partial class MailAttachment 
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		public Guid MailId { get; set; }

		[MaxLength(100)]
		public string Name { get; set; }

		public Stream ContentStream { get; set; }

		[MaxLength(32)]
		public string ContentType { get; set; }

		[Index(IsUnique = false, IsClustered = true)]
		public DateTime CreatedTime { get; set; }

		
		public virtual MailRecord Mail { get; set; }
	}
}
