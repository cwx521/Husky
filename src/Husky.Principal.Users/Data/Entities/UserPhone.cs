using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.Principal.Users.Data
{
	public class UserPhone
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int UserId { get; set; }

		[StringLength(11), Column(TypeName = "varchar(11)"), Required, Phone, Unique]
		public string Number { get; set; } = null!;

		public bool IsVerified { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; init; } = DateTime.Now;


		// nav props

		public User User { get; set; } = null!;
	}
}
