using System.Threading.Tasks;
using Husky.Authentication.Abstractions;
using Husky.Injection;
using Husky.TwoFactor.Data;
using Microsoft.AspNetCore.Mvc;

namespace Insider.Portal.Controllers
{
	public class HomeController : Controller
	{
		public HomeController(IPrincipal principal) {
			_my = principal;
		}

		readonly IPrincipal _my;

		public async Task<IActionResult> Index() {
			await _my.TwoFactor().SendMeTwoFactorCode("chenwx521@hotmail.com", TwoFactorPurpose.ExistenceCheck);
			return View();
		}
	}
}
