using System.Threading.Tasks;
using Husky.Authentication.Abstractions;
using Husky.Injection;
using Husky.Sugar;
using Husky.TwoFactor.Data;
using Husky.Users.Data;
using Insider.Portal.Models.AccountModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Insider.Portal.Controllers
{
	public class ApiController : Controller
	{
		public ApiController(IPrincipal principal, UserDbContext userDb) {
			_my = principal;
			_userDb = userDb;
		}

		readonly IPrincipal _my;
		readonly UserDbContext _userDb;

		[HttpPost]
		public async Task<IActionResult> IsAccountApplicable(RegistryModel model) {
			return Json(!(model.AccountNameType == AccountNameType.Email
				? await _userDb.Users.AnyAsync(x => x.Email == model.AccountName)
				: await _userDb.Users.AnyAsync(x => x.Mobile == model.AccountName)));
		}

		[HttpPost]
		public async Task<IActionResult> SendTwoFactorCode(string to, TwoFactorPurpose purpose) {
			return Json(await _my.TwoFactor().RequestTwoFactorCode(to, purpose));
		}

		[HttpPost]
		public async Task<IActionResult> IsTwoFactorCodeValid(RegistryVerifyModel model) {
			return Json(await _my.TwoFactor().VerifyTwoFactorCode(model.AccountName, model.TwoFactorPurpose, model.TwoFactorCode));
		}
	}
}


