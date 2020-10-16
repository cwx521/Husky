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

		[StringLength(6), Column(TypeName = "varchar(6)")]
		public string HttpMethod { get; set; } = null!;

		public int? UserId { get; set; }

		[StringLength(100)]
		public string? UserName { get; set; }

		public bool IsAjax { get; set; }

		[StringLength(1000), Column(TypeName = "varchar(1000)")]
		public string? UserAgent { get; set; }

		[StringLength(39), Column(TypeName = "varchar(39)")]
		public string? UserIp { get; set; }

		[StringLength(32), Column(TypeName = "varchar(32)"), Unique]
		public string Md5Comparison { get; set; } = null!;

		public int Repeated { get; set; } = 1;

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime FirstTime { get; set; } = DateTime.Now;

		[DefaultValueSql("getdate()")]
		public DateTime LastTime { get; set; } = DateTime.Now;


		internal void ComputeMd5Comparison() => Md5Comparison = Crypto.MD5(string.Concat(
			HttpMethod,
			Url,
			Data,
			UserId,
			IsAjax,
			UserAgent,
			UserIp
		));
	}
}
