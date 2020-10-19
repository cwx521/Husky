using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace Husky.Diagnostics.Data
{
	public class OperationLog : RepeatedLogBase
	{
		public int Id { get; set; }

		public LogLevel LogLevel { get; set; }

		[StringLength(200)]
		public string Message { get; set; } = null!;


		public override void ComputeMd5Comparison() => Md5Comparison = Crypto.MD5(string.Concat(
			AnonymousId,
			UserId,
			LogLevel,
			Message
		));
	}
}
