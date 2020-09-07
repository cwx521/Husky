using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Husky;

namespace Husky.CommonModules.Users.Data
{
	public class User
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(36)]
		public string? DisplayName { get; set; }

		[MaxLength(500), Column(TypeName = "varchar(500)")]
		public string? PhotoUrl { get; set; }

		public RowStatus State { get; set; } = RowStatus.Active;

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime RegisteredTime { get; set; } = DateTime.Now;


		// nav props

		public UserPhone? Phone { get; set; }
		public UserWeChat? WeChat { get; set; }
		public List<UserPassword> Passwords { get; set; } = new List<UserPassword>();
		public List<UserCredit> Credits { get; set; } = new List<UserCredit>();
	}
}
