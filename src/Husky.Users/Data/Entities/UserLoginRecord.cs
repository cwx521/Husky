using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Husky.Data.ModelBuilding.Annotations;

namespace Husky.Users.Data
{
	public class UserLoginRecord
	{
		[Key]
		public long Id { get; set; }

		public Guid? UserId { get; set; }

		[MaxLength(50)]
		public string InputAccount { get; set; }

		[MaxLength(18)]
		public string SickPassword { get; set; }

		public LoginResult LoginResult { get; set; }

		[MaxLength(100)]
		public string Description { get; set; }

		[MaxLength(500)]
		public string UserAgent { get; set; }

		[MaxLength(40), Column(TypeName = "varchar(40)")]
		public string Ip { get; set; }

		[Index(IsClustered = true, IsUnique = false)]
		public DateTime CreateTime { get; set; } = DateTime.Now;


		public User User { get; set; }
	}
}
