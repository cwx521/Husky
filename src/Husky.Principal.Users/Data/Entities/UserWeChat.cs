using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.Principal.Users.Data
{
	public class UserWeChat
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int UserId { get; set; }

		[StringLength(32), Column(TypeName = "varchar(32)")]
		public string? PrivateId { get; set; }

		[StringLength(32), Column(TypeName = "varchar(32)"), Unique]
		public string? UnionId { get; set; }

		[StringLength(36), Required]
		public string NickName { get; set; } = null!;

		public Sex Sex { get; set; }

		[StringLength(500), Column(TypeName = "varchar(500)"), Required]
		public string HeadImageUrl { get; set; } = null!;

		[StringLength(24)]
		public string? Country { get; set; }

		[StringLength(24)]
		public string? Province { get; set; }

		[StringLength(24)]
		public string? City { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		// nav props

		public User User { get; set; } = null!;
		public List<UserWeChatOpenId> OpenIds { get; set; } = new List<UserWeChatOpenId>();
	}
}
