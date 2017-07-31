using Husky.Authentication.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Insider.Portal.Controllers
{
	public class HomeController : Controller
	{
		public HomeController(IPrincipal principal) {
			_my = principal;
		}

		readonly IPrincipal _my;

		public IActionResult Index() {
			return View();
		}
	}
}
