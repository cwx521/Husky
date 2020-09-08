using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.Diagnostics.Data
{
	public class RequestLog
	{
		public int Id { get; set; }

		[MaxLength(1000)]
		public string Url { get; set; } = null!;

		[MaxLength(1000)]
		public string? Referrer { get; set; }

		public string? Data { get; set; }

		[MaxLength(6), Column(TypeName = "varchar(6)")]
		public string HttpMethod { get; set; } = null!;

		public int? UserId { get; set; }

		[MaxLength(100)]
		public string? UserName { get; set; }

		public bool IsAjax { get; set; }

		[MaxLength(1000), Column(TypeName = "varchar(1000)")]
		public string? UserAgent { get; set; }

		[MaxLength(39), Column(TypeName = "varchar(39)")]
		public string? UserIp { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime Time { get; set; } = DateTime.Now;
	}
}
