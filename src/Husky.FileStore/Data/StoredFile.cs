using System;
using System.ComponentModel.DataAnnotations;

namespace Husky.FileStore.Data
{
	public class StoredFile
	{
		[Key]
		public int Id { get; set; }

		public Guid? AnonymousId { get; set; }

		public int? UserId { get; set; }

		[StringLength(50)]
		public string? UserName { get; set; }

		[StringLength(200), Required, NeverUpdate, Unique]
		public string FileName { get; set; } = null!;

		[StringLength(500)]
		public Uri? FileUri { get; set; }

		public long FileContentLength { get; set; }

		public StoredFileType FileType { get; set; }

		public StoredFileAt StoredAt { get; set; }

		public bool IsDeleted { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; set; } = DateTime.Now;
	}
}
