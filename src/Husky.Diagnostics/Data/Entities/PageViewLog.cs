using System.ComponentModel.DataAnnotations;

namespace Husky.Diagnostics.Data
{
	public class PageViewLog : LogBase
	{
		public int Id { get; set; }

		[StringLength(50)]
		public string PageId { get; set; } = null!;
	}
}
