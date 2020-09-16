using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Husky.Users.Data;

namespace Husky.Payments.Data
{
	public class PayDeposit
	{
		[Key]
		public int Id { get; set; }

		public int UserId { get; set; }

		public int CreditTypeId { get; set; }

		public PaymentChannel Channel { get; set; }

		[Column(TypeName = "decimal(15, 8)")]
		public decimal Amount { get; set; }

		public bool HasRefund { get; set; }

		[Column(TypeName = "varchar(15)"), Unique]
		public string OrderId { get; set; } = null!;

		[Column(TypeName = "varchar(64)"), Unique]
		public string? ExternalTradeId { get; set; }

		[Column(TypeName = "varchar(32)")]
		public string? ExternalUserId { get; set; }

		[Column(TypeName = "varchar(50)")]
		public string? ExternalUserName { get; set; }

		[MaxLength(1000)]
		public string? PaymentLoader { get; set; }

		[MaxLength(200)]
		public string? Remarks { get; set; }

		public DepositStatus Status { get; set; } = DepositStatus.AwaitPay;

		public DateTime? StatusChangedTime { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		// nav props

		public User User { get; set; } = null!;
		public CreditType CreditType { get; set; } = null!;
		public List<PayRefund> Refunds { get; set; } = new List<PayRefund>();
	}
}
