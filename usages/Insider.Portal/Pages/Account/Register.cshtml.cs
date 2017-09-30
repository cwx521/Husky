using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Husky.Authentication.Abstractions;
using Husky.Injection;
using Husky.Sugar;
using Insider.Portal.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Insider.Portal.Pages.Account
{
	public class RegisterModel : PageModel
	{
		public RegisterModel(IPrincipal principal) {
			_my = principal;
		}

		readonly IPrincipal _my;

		[BindProperty]
		public RegisterAccountModel Account { get; set; }

		[TempData]
		public string AccountName { get; set; }
		[TempData]
		public bool AutoSend { get; set; } = true;

		public void OnGet() {
		}

		public async Task<IActionResult> OnPostAsync() {
			if ( ModelState.IsValid ) {
				var result = await _my.User().SignUp(Account.Type, Account.Name, Account.Password, verified: false);
				if ( result.Ok ) {
					AccountName = Account.Name;
					AutoSend = true;
					return RedirectToPage("Verify");
				}
				ModelState.AddModelError(nameof(Account.Name), result.Message);
			}
			return Page();
		}
	}

	public class RegisterAccountModel
	{
		const string _typeName = "邮箱";
		public AccountNameType Type => AccountNameType.Email;

		[Required(ErrorMessage = "必须填写，请用您的" + _typeName + "作为帐号名。")]
		[EmailAddress(ErrorMessage = "格式无效，请用您的" + _typeName + "作为帐号名。")]
		[RegularExpression(StringTest.EmailRegexPattern, ErrorMessage = "格式无效，请用您的" + _typeName + "作为帐号名。")]
		[Remote(nameof(ApiController.IsAccountApplicable), "Api", AdditionalFields = nameof(Type), HttpMethod = "POST", ErrorMessage = "{0}已经被注册了。")]
		[Display(Name = _typeName)]
		public string Name { get; set; }

		[Required(ErrorMessage = "密码必须填写。")]
		[StringLength(18, MinimumLength = 8, ErrorMessage = "密码长度须在{2}-{1}位之间。")]
		[DataType(DataType.Password)]
		[Display(Name = "密码")]
		public string Password { get; set; }

		[Required(ErrorMessage = "重复输入一遍密码。")]
		[MaxLength(15), Compare(nameof(Password), ErrorMessage = "两次密码输入不一致。")]
		[DataType(DataType.Password)]
		[Display(Name = "密码确认")]
		public string PasswordRepeat { get; set; }
	}
}