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
		[Column(TypeName = "varchar(36)")]
		public string UserIdString { get; set; }

		[NeverUpdate]
		[Column(TypeName = "varchar(50)")]
		public string SentTo { get; set; }

		[NeverUpdate]
		[Column(TypeName = "varchar(8)")]
		public string Code { get; set; }

		public int ErrorTimes { get; set; }

		public bool IsUsed { get; set; }

		[NeverUpdate]
		public DateTime CreatedTime { get; set; } = DateTime.Now;
	}
}
