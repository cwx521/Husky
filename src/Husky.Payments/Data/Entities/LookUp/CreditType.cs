using System.ComponentModel.DataAnnotations;

namespace Husky.Payments.Data
{
	public class CreditType
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(10), Unique]
		public string CreditName { get; set; } = null!;

		[MaxLength(10)]
		public string Unit { get; set; } = null!;
	}
}