using System.Threading.Tasks;
using Husky.Authentication.Abstractions;
using Husky.Injection;
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

		[HttpGet("~/register")]
		public IActionResult Register() => View();

		[HttpPost("~/register")]
		public async Task<IActionResult> Register(RegistryModel model) {
			if ( ModelState.IsValid ) {
				var result = await _my.User().SignUp(model.AccountNameType, model.AccountName, model.Password, verified: false);
				if ( result.Ok ) {
					return View(nameof(Verify), new RegistryVerifyModel { AccountName = model.AccountName });
				}
				ModelState.AddModelError(nameof(model.AccountName), result.Message);
			}
			return View(model);
		}

		[HttpPost("~/register/verify")]
		public IActionResult Verify(RegistryVerifyModel model) {
			return View(model);
		}
	}
}
