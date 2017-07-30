using System.Threading.Tasks;
using Husky.Authentication.Abstractions;
using Husky.TwoFactor;
using Husky.TwoFactor.Data;
using Husky.Users.Data;
using Microsoft.AspNetCore.Mvc;

namespace Insider.Portal.Controllers
{
	public class HomeController : Controller
	{
		public HomeController(IPrincipal principal, UserDbContext userDb) {
			_my = principal;
			_userDb = userDb;
		}

		readonly IPrincipal _my;
		readonly UserDbContext _userDb;

		public async Task<IActionResult> Index() {
			await _my.TwoFactor().SendMeTwoFactorCode("chenwx521@hotmail.com", TwoFactorPurpose.ExistenceCheck);
			return View();
		}
	}
}
