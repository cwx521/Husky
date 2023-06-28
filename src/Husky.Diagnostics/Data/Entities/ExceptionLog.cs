using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Husky.Diagnostics.Data
{
	[Index(nameof(Md5Comparison), IsUnique = false)]
	public class ExceptionLog : HttpLevelLogBase
	{
		[Key]
		public int Id { get; set; }

		[StringLength(50), Unicode(false), Required]
		public string ExceptionType { get; set; } = null!;

		[StringLength(1000)]
		public string? Message { get; set; }

		public string? Source { get; set; }

		public string? StackTrace { get; set; }


		public override void ComputeMd5Comparison() => Md5Comparison = Crypto.MD5(string.Concat(
			HttpMethod,
			ExceptionType,
			Message,
			Source,
			StackTrace,
			UserId == null || UserId.Value == 0
		));
	}
}
