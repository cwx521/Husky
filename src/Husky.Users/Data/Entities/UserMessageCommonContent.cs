using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Husky.Users.Data
{
	public class UserMessageCommonContent
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(4000)]
		public string Content { get; set; } = null!;

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		// nav props

		[JsonIgnore]
		public List<UserMessage> UserMessages { get; set; } = new List<UserMessage>();
	}
}
