using System.ComponentModel.DataAnnotations;

namespace Husky.CommonModules.Users.Data
{
	public class CreditType
	{
		[Key]
		public int Id { get; set; }

		[StringLength(10)]
		public string CreditName { get; set; } = null!;

		[StringLength(10)]
		public string? Unit { get; set; }
	}
}
