using System;
using Husky.Authentication;
using Husky.Authentication.Abstractions;
using Husky.Users.Data;
using Microsoft.AspNetCore.Mvc;

namespace Insider.Portal.Controllers
{
	public class HomeController : Controller
	{
		public HomeController(Principal<Guid> principal, UserDbContext userDb) {
			_my = principal;
			_userDb = userDb;
		}

		readonly Principal<Guid> _my;
		readonly UserDbContext _userDb;

		public IActionResult Index() {
			return View();
		}
	}
}
