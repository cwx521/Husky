using System.ComponentModel.DataAnnotations;
using Husky.Authentication.Abstractions;
using Husky.Injection;
using Husky.Sugar;
using Husky.Users.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Insider.Portal.Pages
{
	public class LoginModel : PageModel
	{
		public LoginModel(IPrincipal principal) {
			_my = principal;
		}

		readonly IPrincipal _my;

		[Required(ErrorMessage = "请填写登录名，可以是可使用邮箱或手机号。")]
		[RegularExpression(@"^([-0-9a-zA-Z.+_]+@[-0-9a-zA-Z.+_]+\.[a-zA-Z]{2,4})|(1[3578]\d{9})$", ErrorMessage = "格式无效，仅可使用邮箱或手机号作为用户名。")]
		[Display(Name = "登录帐号")]
		public string AccountName { get; set; }

		[Required(ErrorMessage = "请填写密码。")]
		[StringLength(18, MinimumLength = 8, ErrorMessage = "密码长度须在{2}-{1}位之间。")]
		[DataType(DataType.Password)]
		[Display(Name = "密码")]
		public string Password { get; set; }

		public void OnGet()
        {
        }

		public async void OnPost() {
			if ( ModelState.IsValid ) {
				var result = await _my.User().SignIn(AccountName, Password);
				if ( result == LoginResult.Success ) {
					Redirect("/");
				}
				ModelState.AddModelError("", result.ToLabel());
			}
		}
	}
}