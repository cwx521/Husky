using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.Diagnostics.Data
{
	public abstract class LogBase
	{
		public Guid? AnonymousId { get; set; }

		public int? UserId { get; set; }

		[StringLength(100)]
		public string? UserName { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime FirstTime { get; set; } = DateTime.Now;

		[DefaultValueSql("getdate()")]
		public DateTime LastTime { get; set; } = DateTime.Now;
	}
}
