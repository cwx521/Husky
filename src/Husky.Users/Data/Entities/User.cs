using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Husky.Data.ModelBuilding.Annotations;
using Husky.Sugar;

namespace Husky.Users.Data
{
	public class User
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		[MaxLength(32)]
		public string DisplayName { get; set; }

		[MaxLength(15), Index(IsUnique = true), Column(TypeName = "varchar(15)")]
		public string Mobile { get; set; }

		[MaxLength(40), Index(IsUnique = true), Column(TypeName = "varchar(40)")]
		public string Email { get; set; }

		[MaxLength(40), Column(TypeName = "varchar(40)")]
		public string Password { get; set; }

		public bool IsEmailVerified { get; set; }

		public bool IsMobileVerified { get; set; }

		public RowStatus Status { get; set; } = RowStatus.Active;

		public DateTime? AwaitReactivateTime { get; set; }

		[Index(IsClustered = true, IsUnique = false)]
		public DateTime CreatedTime { get; set; } = DateTime.Now;



		public UserPersonal Personal { get; set; }
		public List<UserLoginRecord> LoginRecords { get; set; }
		public List<UserChangeRecord> ChangeRecords { get; set; }
	}
}
