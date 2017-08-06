using System.Threading.Tasks;
using Husky.Authentication.Abstractions;
using Husky.Injection;
using Husky.Sugar;
using Husky.TwoFactor.Data;
using Husky.Users.Data;
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
					return View(nameof(Verify), new RegistryVerifyModel { AccountName = model.AccountName, AutoSend = true });
				}
				ModelState.AddModelError(nameof(model.AccountName), result.Message);
			}
			return View(model);
		}

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

		[HttpGet("~/logout")]
		public IActionResult Logout() {
			_my.User().SignOut();
			return View();
		}

		[HttpGet("~/login")]
		public IActionResult Login() => View();

		[HttpPost("~/login")]
		public async Task<IActionResult> Login(LoginModel model) {
			if ( ModelState.IsValid ) {
				var result = await _my.User().SignIn(model.AccountName, model.Password);
				if ( result == LoginResult.Success ) {
					return Redirect("/");
				}
				ModelState.AddModelError("", result.ToLabel());
			}
			return View(model);
		}
	}
}
