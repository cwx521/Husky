using System.ComponentModel.DataAnnotations;

namespace Husky.Diagnostics.Data
{
	public class PageViewLog : HttpLevelLogBase
	{
		public int Id { get; set; }

		[StringLength(50)]
		public string PageName { get; set; } = null!;

		[StringLength(200)]
		public string? Description { get; set; }
	}
}
