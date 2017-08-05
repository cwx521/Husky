using System.ComponentModel.DataAnnotations;

namespace Insider.Portal.Models.AccountModels
{
	public class LoginModel
	{
		[Required(ErrorMessage = "请填写登录名。")]
		[RegularExpression(@"^([-0-9a-zA-Z.+_]+@[-0-9a-zA-Z.+_]+\.[a-zA-Z]{2,4})|(1[3578]\d{9})$", ErrorMessage = "格式无效，仅可使用邮箱或手机号作为用户名。")]
		[Display(Name = "登录帐号")]
		public string AccountName { get; set; }

		[Required(ErrorMessage = "请填写密码。")]
		[StringLength(18, MinimumLength = 8, ErrorMessage = "密码长度须在{2}-{1}位之间。")]
		[DataType(DataType.Password)]
		[Display(Name = "密码")]
		public string Password { get; set; }
	}
}
