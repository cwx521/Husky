using System;
using System.ComponentModel.DataAnnotations;
using Husky.Data.ModelBuilding.Annotations;

namespace Husky.Users.Data
{
	public class UserChangeRecord
	{
		[Key]
		public long Id { get; set; }

		public Guid? UserId { get; set; }

		[MaxLength(50)]
		public string FieldName { get; set; }

		[MaxLength(100)]
		public string OldValue { get; set; }

		[MaxLength(100)]
		public string NewValue { get; set; }

		[MaxLength(100)]
		public string Description { get; set; }

		public bool IsBySelf { get; set; }

		[Index(IsClustered = true, IsUnique = false)]
		public DateTime CreateTime { get; set; } = DateTime.Now;


		public User User { get; set; }
	}
}
