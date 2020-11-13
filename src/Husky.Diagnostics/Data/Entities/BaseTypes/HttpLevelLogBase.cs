using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace Husky.Diagnostics.Data
{
	public abstract class HttpLevelLogBase : RepeatedLogBase
	{
		[StringLength(6), Column(TypeName = "varchar(6)")]
		public string? HttpMethod { get; set; }

		[StringLength(1000)]
		public string? Url { get; set; }

		[StringLength(1000)]
		public string? Referrer { get; set; }

		public string? Data { get; set; }

		[StringLength(1000), Column(TypeName = "varchar(1000)")]
		public string? UserAgent { get; set; }

		[Column(TypeName = "varchar(45)")]
		public IPAddress? UserIp { get; set; }

		public bool IsAjax { get; set; }
	}
}
