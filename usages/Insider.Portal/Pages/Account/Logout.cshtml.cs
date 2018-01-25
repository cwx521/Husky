using Husky.Authentication.Abstractions;
using Husky.Injection;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Insider.Portal.Pages.Account
{
	public class LogoutModel : PageModel
	{
		public LogoutModel(IPrincipal principal) {
			_my = principal;
		}

		readonly IPrincipal _my;

		public void OnGet() {
			_my.User().SignOut();
		}
	}
}