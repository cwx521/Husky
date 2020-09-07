using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Husky.CommonModules.Users.Data
{
	public class UserPassword
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		[MaxLength(40), Column(TypeName = "varchar(40)")]
		public string Password { get; set; } = null!;

		public bool IsObsoleted { get; set; }

		public DateTime CreatedTime { get; set; } = DateTime.Now;


		// nav props

		[JsonIgnore]
		public User User { get; set; } = null!;
	}
}
