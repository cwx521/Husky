using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Husky.Users.Data;

namespace Husky.Payments.Data
{
	public class PayWithdrawal
	{
		[Key]
		public int Id { get; set; }

		public int UserId { get; set; }

		public int CreditTypeId { get; set; }

		[Column(TypeName = "decimal(10, 2)")]
		public decimal NetAmount { get; set; }

		[Column(TypeName = "decimal(10, 2)")]
		public decimal HandlingFee { get; set; }

		[Required, Column(TypeName = "varchar(15)"), Unique]
		public string OrderId { get; set; } = null!;

		[Column(TypeName = "varchar(64)"), Unique]
		public string? ExternalTradeId { get; set; }

		public PaymentChannel TargetChannel { get; set; }

		[MaxLength(50)]
		public string TargetAccount { get; set; } = null!;

		[MaxLength(50)]
		public string TargetAccountAlias { get; set; } = null!;

		[MaxLength(200)]
		public string? Remarks { get; set; }

		public WithdrawalStatus Status { get; set; } = WithdrawalStatus.New;

		public DateTime? StatusChangedTime { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		// nav props

		public User User { get; set; } = null!;
		public CreditType CreditType { get; set; } = null!;


		// nav prop

		public decimal Amount => NetAmount + HandlingFee;
	}
}
