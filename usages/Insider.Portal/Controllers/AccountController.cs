using System.Threading.Tasks;
using Husky.Authentication.Abstractions;
using Husky.Injection;
using Husky.TwoFactor.Data;
using Insider.Portal.Models.AccountModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Insider.Portal.Controllers
{
	[AllowAnonymous]
	public class AccountController : Controller
	{
		public AccountController(IPrincipal principal) {
			_my = principal;
		}

		readonly IPrincipal _my;

		[HttpPost("~/register/verify")]
		public async Task<IActionResult> Verify(RegistryVerifyModel model) {
			if ( ModelState.IsValid ) {
				var result = await _my.TwoFactor().VerifyTwoFactorCode(model.AccountName, TwoFactorPurpose.Registry, model.TwoFactorCode, true);
				if ( result.Ok ) {
					return Redirect("/");
				}
				ModelState.AddModelError(nameof(model.TwoFactorCode), result.Message);
			}
			return View(model);
		}
	}
}
