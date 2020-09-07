using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Husky.CommonModules.Users.Data
{
	public class UserMessageCommonContent
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(4000)]
		public string Content { get; set; } = null!;


		// nav props

		public List<UserMessage> UserMessages { get; set; } = new List<UserMessage>();
	}
}
