using Husky.Authentication.Abstractions;
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

		public IActionResult Index() {
			return View();
		}
	}
}
