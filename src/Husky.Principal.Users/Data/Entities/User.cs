using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.Principal.Users.Data
{
	public class User
	{
		[Key]
		public int Id { get; set; }

		[StringLength(36)]
		public string? DisplayName { get; set; }

		[StringLength(500), Column(TypeName = "varchar(500)")]
		public string? PhotoUrl { get; set; }

		public RowStatus Status { get; set; } = RowStatus.Active;

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime RegisteredTime { get; set; } = DateTime.Now;


		// nav props

		public UserPhone? Phone { get; set; }
		public UserEmail? Email { get; set; }
		public UserWeChat? WeChat { get; set; }
		public UserReal? Real { get; set; }
		public List<UserPassword> Passwords { get; set; } = new List<UserPassword>();
		public List<UserInGroup> InGroups { get; set; } = new List<UserInGroup>();
		public List<UserAddress> Addresses { get; set; } = new List<UserAddress>();
	}
}
