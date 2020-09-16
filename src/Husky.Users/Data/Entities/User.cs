using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.Users.Data
{
	public class User
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(36)]
		public string? DisplayName { get; set; }

		[MaxLength(500), Column(TypeName = "varchar(500)")]
		public string? PhotoUrl { get; set; }

		public RowStatus Status { get; set; } = RowStatus.Active;

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime RegisteredTime { get; set; } = DateTime.Now;


		// nav props

		public UserPhone? Phone { get; set; }
		public UserWeChat? WeChat { get; set; }
		public UserReal? Real { get; set; }
		public List<UserPassword> Passwords { get; set; } = new List<UserPassword>();
	}
}
