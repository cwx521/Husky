using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace Husky.Diagnostics.Data
{
	public class PageViewLog : LogBase
	{
		public int Id { get; set; }

		[StringLength(50)]
		public string PageId { get; set; } = null!;

		[StringLength(500)]
		public string? Referer { get; set; }

		[StringLength(500), Unicode(false)]
		public string? UserAgent { get; set; }

		[StringLength(45), Unicode(false)]
		public IPAddress? UserIp { get; set; }
	}
}
