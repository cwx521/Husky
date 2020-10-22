using System;
using System.ComponentModel.DataAnnotations;

namespace Husky.Diagnostics.Data
{
	public abstract class LogBase
	{
		public Guid? AnonymousId { get; set; }

		public int? UserId { get; set; }

		[StringLength(100)]
		public string? UserName { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime Time { get; set; } = DateTime.Now;
	}
}
