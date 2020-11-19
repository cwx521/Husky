using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.Principal.Users.Data
{
	public class UserEmail
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int UserId { get; set; }

		[StringLength(50), Column(TypeName = "varchar(50)"), Required, EmailAddress, Unique]
		public string EmailAddress { get; set; } = null!;

		public bool IsVerified { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; init; } = DateTime.Now;


		// nav props

		public User User { get; set; } = null!;
	}
}
