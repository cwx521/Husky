using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace Husky.Principal.Users.Data
{
	public class UserLoginRecord
	{
		[Key]
		public int Id { get; set; }

		public int? UserId { get; set; }

		[StringLength(50), Required]
		public string AttemptedAccount { get; set; } = null!;

		[StringLength(88), Column(TypeName = "varchar(88)")]
		public string? SickPassword { get; set; }

		public LoginResult LoginResult { get; set; }

		[StringLength(500)]
		public string? UserAgent { get; set; }

		[Column(TypeName = "varchar(45)")]
		public IPAddress? Ip { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; init; } = DateTime.Now;


		// nav props

		public User? User { get; set; }
	}
}
