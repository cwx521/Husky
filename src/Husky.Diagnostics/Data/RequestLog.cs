using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.Diagnostics.Data
{
	public class RequestLog
	{
		public int Id { get; set; }

		[StringLength(1000)]
		public string Url { get; set; } = null!;

		[StringLength(1000)]
		public string? Referrer { get; set; }

		public string? Data { get; set; }

		[StringLength(10), Column(TypeName = "varchar(10)")]
		public string HttpMethod { get; set; } = null!;

		public int? UserId { get; set; }

		[StringLength(100)]
		public string? UserName { get; set; }

		public bool IsAjax { get; set; }

		[StringLength(1000), Column(TypeName = "varchar(1000)")]
		public string? UserAgent { get; set; }

		[StringLength(39), Column(TypeName = "varchar(39)")]
		public string? UserIp { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime Time { get; set; } = DateTime.Now;
	}
}
