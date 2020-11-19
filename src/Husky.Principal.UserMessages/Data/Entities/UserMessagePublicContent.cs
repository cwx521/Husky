using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Husky.Principal.UserMessages.Data
{
	public class UserMessagePublicContent
	{
		[Key]
		public int Id { get; set; }

		[StringLength(4000), Required]
		public string Content { get; set; } = null!;

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; init; } = DateTime.Now;


		// nav props

		public List<UserMessage> UserMessages { get; set; } = new List<UserMessage>();
	}
}
