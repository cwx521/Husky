using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Husky.Users.Data
{
	public class UserReal
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int UserId { get; set; }

		[MaxLength(18), Column(TypeName = "varchar(11)"), Index(IsUnique = true)]
		public string? SocialIdNumber { get; set; }

		[Required, MaxLength(24)]
		public string? RealName { get; set; }

		[Column(TypeName = "varchar(60)")]
		public string? RealNamePhonetic { get; set; }

		[Column(TypeName = "varchar(10)")]
		public string? RealNamePhoneticInitials { get; set; }

		public Sex? Sex { get; set; }

		public bool IsVerified { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		// nav props

		[JsonIgnore]
		public User User { get; set; } = null!;
	}
}
