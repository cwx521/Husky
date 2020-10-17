using System;
using System.ComponentModel.DataAnnotations;

namespace Husky.Principal.Users.Data
{
	public class UserMessage
	{
		[Key]
		public int Id { get; set; }

		public int UserId { get; set; }

		public int? PublicContentId { get; set; }

		[StringLength(4000)]
		public string? Content { get; set; }

		public bool IsRead { get; set; }

		public bool IsDeleted { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		// nav props

		public User User { get; set; } = null!;
		public UserMessagePublicContent? PublicContent { get; set; }
	}
}
