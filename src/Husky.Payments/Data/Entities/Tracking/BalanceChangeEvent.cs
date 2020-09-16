using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.Payments.Data
{
	public class BalanceChangeEvent
	{
		[Key]
		public int Id { get; set; }

		[Column(TypeName = "varchar(16)"), Unique]
		public string OrderId { get; set; } = null!;

		[MaxLength(200)]
		public string? Description { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime Time { get; set; } = DateTime.Now;


		// nav props

		public List<BalanceChange> BalanceChanges { get; set; } = new List<BalanceChange>();
	}
}
