using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.Payments.Data
{
	public class PayRefund
	{
		public int Id { get; set; }

		public int DepositId { get; set; }

		[Column(TypeName = "varchar(15)"), Unique]
		public string RefundRequestOrderId { get; set; } = null!;

		public int CreditTypeId { get; set; }

		public decimal RefundAmount { get; set; }

		[MaxLength(200)]
		public string? Reason { get; set; }

		[MaxLength(200)]
		public string? Remarks { get; set; }

		public RefundStatus Status { get; set; } = RefundStatus.New;

		public DateTime? StatusChangedTime { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		// nav props

		public PayDeposit Deposit { get; set; } = null!;
		public CreditType CreditType { get; set; } = null!;
	}
}
