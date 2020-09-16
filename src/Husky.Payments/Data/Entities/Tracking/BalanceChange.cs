using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Husky.Users.Data;

namespace Husky.Payments.Data
{
	public class BalanceChange
	{
		[Key]
		public int Id { get; set; }

		public int UserId { get; set; }

		public int CreditTypeId { get; set; }

		public int EventId { get; set; }

		[Column(TypeName = "decimal(10, 2)")]
		public decimal Amount { get; set; }

		[Column(TypeName = "decimal(10, 2)")]
		public decimal BalanceSnapshot { get; set; }

		public BalanceChangeReason Reason { get; set; }


		// nav props

		public User User { get; set; } = null!;

		[JsonIgnore]
		public BalanceChangeEvent Event { get; set; } = null!;

		public CreditType CreditType { get; set; } = null!;
	}
}
