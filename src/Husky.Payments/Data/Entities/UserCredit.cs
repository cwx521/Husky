using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Husky.Users.Data;
using Newtonsoft.Json;

namespace Husky.Payments.Data
{
	public class UserCredit
	{
		[Key]
		public int Id { get; set; }

		[CompositeUnique, NeverUpdate]
		public int UserId { get; set; }

		[CompositeUnique, NeverUpdate]
		public int CreditTypeId { get; set; }

		[Column(TypeName = "decimal(8, 2)")]
		public decimal Balance { get; set; }


		// nav props

		[JsonIgnore]
		public User User { get; set; } = null!;

		public CreditType CreditType { get; set; } = null!;
	}
}
