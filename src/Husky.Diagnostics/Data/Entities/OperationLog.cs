using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace Husky.Diagnostics.Data
{
	public class OperationLog : LogBase
	{
		public int Id { get; set; }

		public LogLevel LogLevel { get; set; }

		[StringLength(200)]
		public string Message { get; set; } = null!;
	}
}
