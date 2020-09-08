using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.TwoFactor.Data
{
	public class TwoFactorCode
	{
		[Key]
		public int Id { get; set; }

		[NeverUpdate]
		public int UserId { get; set; }

		[NeverUpdate]
		[Column(TypeName = "varchar(50)")]
		public string SentTo { get; set; } = null!;

		[NeverUpdate]
		[Column(TypeName = "varchar(8)")]
		public string Code { get; set; } = null!;

		public int ErrorTimes { get; set; }

		public bool IsUsed { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; set; } = DateTime.Now;
	}
}
