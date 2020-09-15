using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.Diagnostics.Data
{
	public class ExceptionLog
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(32), Column(TypeName = "varchar(32)")]
		public string Md5Comparison { get; set; } = null!;

		[MaxLength(4000), Column(TypeName = "varchar(4000)")]
		public string? Url { get; set; }

		[MaxLength(6), Column(TypeName = "varchar(6)")]
		public string? HttpMethod { get; set; }

		[MaxLength(200), Column(TypeName = "varchar(200)")]
		public string ExceptionType { get; set; } = null!;

		[MaxLength(1000)]
		public string? Message { get; set; }

		public string? Source { get; set; }

		public string? StackTrace { get; set; }

		public int? UserId { get; set; }

		[MaxLength(100)]
		public string? UserName { get; set; }

		[MaxLength(1000), Column(TypeName = "varchar(1000)")]
		public string? UserAgent { get; set; }

		[MaxLength(39), Column(TypeName = "varchar(39)")]
		public string? UserIp { get; set; }

		public int Count { get; set; } = 1;

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime FirstTime { get; set; } = DateTime.Now;

		[DefaultValueSql("getdate()")]
		public DateTime LastTime { get; set; } = DateTime.Now;


		internal void ComputeMd5Comparison() => Md5Comparison = Crypto.MD5(string.Concat(
			HttpMethod,
			ExceptionType,
			Message,
			Source,
			StackTrace
		));
	}
}
