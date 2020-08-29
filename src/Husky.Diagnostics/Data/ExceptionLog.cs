using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.Diagnostics.Data
{
	public class ExceptionLog
	{
		[Key]
		public int Id { get; set; }

		[StringLength(32), Column(TypeName = "varchar(32)")]
		public string Md5Comparison { get; set; }

		[StringLength(4000), Column(TypeName = "varchar(4000)")]
		public string Url { get; set; }

		[StringLength(10), Column(TypeName = "varchar(10)")]
		public string HttpMethod { get; set; }

		[StringLength(200), Column(TypeName = "varchar(200)")]
		public string ExceptionType { get; set; }

		[StringLength(1000)]
		public string Message { get; set; }

		public string Source { get; set; }

		public string StackTrace { get; set; }
		
		[StringLength(36), Column(TypeName = "varchar(36)")]
		public string UserIdString { get; set; }

		[StringLength(100)]
		public string UserName { get; set; }

		[StringLength(1000), Column(TypeName = "varchar(1000)")]
		public string UserAgent { get; set; }

		[StringLength(39), Column(TypeName = "varchar(39)")]
		public string UserIp { get; set; }

		public int Count { get; set; } = 1;

		public DateTime FirstTime { get; set; } = DateTime.Now;

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
