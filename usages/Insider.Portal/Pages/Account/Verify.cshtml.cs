using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Husky.Authentication.Abstractions;
using Husky.Injection;
using Husky.TwoFactor.Data;
using Insider.Portal.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Insider.Portal.Pages.Account
{
	public class VerifyModel : PageModel
	{
		public VerifyModel(IPrincipal principal) {
			_my = principal;
		}

		readonly IPrincipal _my;

		[BindProperty]
		public VerifyDataModel Data { get; set; }

		[TempData]
		public string AccountName { get; set; }
		[TempData]
		public bool AutoSend { get; set; }

		public void OnGet() {
			Data.AccountName = AccountName;
			Data.AutoSend = AutoSend;
		}

		public async Task<IActionResult> OnPostAsync() {
			if ( ModelState.IsValid ) {
				var result = await _my.TwoFactor().VerifyTwoFactorCode(Data.AccountName, TwoFactorPurpose.Registry, Data.TwoFactorCode, true);
				if ( result.Ok ) {
					return Redirect("/");
				}
				ModelState.AddModelError(nameof(Data.TwoFactorCode), result.Message);
			}
			return Page();
		}
	}

	public class VerifyDataModel
	{
		[Display(Name = "帐号")]
		public string AccountName { get; set; }

		[Required(ErrorMessage = "请输入您接收到的验证码。")]
		[RegularExpression(@"^\d{6}$", ErrorMessage = "请输入正确的验证码数字，长度6位。")]
		[Remote(nameof(ApiController.IsTwoFactorCodeValid), "Api", AdditionalFields = nameof(AccountName) + "," + nameof(TwoFactorPurpose), HttpMethod = "POST", ErrorMessage = "验证码不正确。")]
		[Display(Name = "验证码")]
		public string TwoFactorCode { get; set; }

		public bool AutoSend { get; set; }

		public TwoFactorPurpose TwoFactorPurpose => TwoFactorPurpose.Registry;
	}
}