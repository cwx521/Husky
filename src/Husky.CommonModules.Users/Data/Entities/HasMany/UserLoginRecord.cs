using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Husky.CommonModules.Users.Data
{
	public class UserLoginRecord
	{
		[Key]
		public int Id { get; set; }

		public int? UserId { get; set; }

		[MaxLength(50)]
		public string AttemptedAccount { get; set; } = null!;

		[MaxLength(88), Column(TypeName = "varchar(88)")]
		public string? SickPassword { get; set; }

		public LoginResult LoginResult { get; set; }

		[MaxLength(100)]
		public string? Description { get; set; }

		[MaxLength(500)]
		public string? UserAgent { get; set; }

		[MaxLength(39), Column(TypeName = "varchar(39)")]
		public string? Ip { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		// nav props

		[JsonIgnore]
		public User? User { get; set; }
	}
}
