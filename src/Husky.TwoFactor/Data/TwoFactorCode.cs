using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Husky.TwoFactor.Data
{
	public class TwoFactorCode
	{
		[Key]
		public int Id { get; set; }

		[NeverUpdate]
		public Guid AnonymousId { get; set; }

		[NeverUpdate]
		public int UserId { get; set; }

		[NeverUpdate]
		[StringLength(50), Unicode(false), Required]
		public string SentTo { get; set; } = null!;

		[NeverUpdate]
		[StringLength(8), Unicode(false), Required]
		public string Code { get; set; } = null!;

		public int ErrorTimes { get; set; }

		public bool IsUsed { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; init; } = DateTime.Now;
	}
}
