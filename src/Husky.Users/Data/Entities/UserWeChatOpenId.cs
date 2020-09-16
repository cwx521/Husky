using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace Husky.Users.Data
{
	public class UserWeChatOpenId
	{
		[Key]
		public int Id { get; set; }

		public int WeChatId { get; set; }

		public WeChatOpenIdType OpenIdType { get; set; }

		[MaxLength(32), Column(TypeName = "varchar(32)"), Unique]
		public string OpenIdValue { get; set; } = null!;


		// nav props

		[JsonIgnore]
		public UserWeChat WeChat { get; set; } = null!;
	}
}
