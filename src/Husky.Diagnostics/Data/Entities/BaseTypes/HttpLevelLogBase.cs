using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace Husky.Diagnostics.Data
{
	public abstract class HttpLevelLogBase : LogBase
	{
		[StringLength(6), Unicode(false)]
		public string? HttpMethod { get; set; }

		[StringLength(1000)]
		public string? Url { get; set; }

		[StringLength(500)]
		public string? Referer { get; set; }

		public string? Data { get; set; }

		[StringLength(500), Unicode(false)]
		public string? UserAgent { get; set; }

		[StringLength(45), Unicode(false)]
		public IPAddress? UserIp { get; set; }

		public bool IsAjax { get; set; }
	}
}
