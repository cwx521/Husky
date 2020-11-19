using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.Principal.Users.Data
{
	public class UserPassword
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		[StringLength(40), Column(TypeName = "varchar(40)"), Required]
		public string Password { get; set; } = null!;

		public bool IsObsolete { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; init; } = DateTime.Now;


		// nav props

		public User User { get; set; } = null!;
	}
}
