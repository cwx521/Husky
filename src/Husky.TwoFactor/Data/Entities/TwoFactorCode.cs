using System;
using Husky.Data;
using Husky.Sugar;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Husky.Data.ModelBuilding.Annotations;

namespace Husky.TwoFactor.Data
{
	public class TwoFactorCode
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		public Guid? UserId { get; set; }

		[MaxLength(50), Column(TypeName = "varchar(50)")]
		public string SentTo { get; set; }

		[MaxLength(24), Column(TypeName = "varchar(8)")]
		public string PassCode { get; set; }

		public TwoFactorPurpose Purpose { get; set; }

		public bool IsUsed { get; set; }

		[Index(IsClustered = true, IsUnique = false)]
		public DateTime CreatedTime { get; set; } = DateTime.Now;
	}
}
