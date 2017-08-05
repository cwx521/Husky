using System;
using System.ComponentModel.DataAnnotations;
using Husky.Data.ModelBuilding.Annotations;
using Husky.Sugar;

namespace Husky.Users.Data
{
	public class UserPersonal
	{
		[Key]
		public Guid UserId { get; set; }

		[Required, MaxLength(18)]
		public string FirstName { get; set; }

		[Required, MaxLength(18)]
		public string LastName { get; set; }

		[Required, MaxLength(18)]
		public string FirstNamePhonetic { get; set; }

		[Required, MaxLength(18)]
		public string LastNamePhonetic { get; set; }

		public byte[] Photo { get; set; }

		public Sex Sex { get; set; } = Sex.Untold;

		[MaxLength(18)]
		public string Location { get; set; }

		public DateTime? DateOfBirth { get; set; }

		[Index(IsClustered = true, IsUnique = false)]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		public User User { get; set; }
	}
}
