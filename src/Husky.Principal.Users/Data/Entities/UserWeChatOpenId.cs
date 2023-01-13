using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Husky.WeChatIntegration;

namespace Husky.Principal.Users.Data
{
	public class UserWeChatOpenId
	{
		[Key]
		public int Id { get; set; }

		[NeverUpdate]
		public int WeChatId { get; set; }

		public WeChatRegion OpenIdType { get; set; }

		[StringLength(32), Column(TypeName = "varchar(32)"), Required, Unique]
		public string OpenIdValue { get; set; } = null!;


		// nav props

		public UserWeChat WeChat { get; set; } = null!;
	}
}
