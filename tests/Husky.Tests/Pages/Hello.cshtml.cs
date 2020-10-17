using Husky.Principal;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Husky.Tests.Pages
{
	public class HelloPageModel : PageModel
	{
		public HelloPageModel(IPrincipalUser principal) {
			_me = principal;
		}

		private IPrincipalUser _me;

		public void OnGet() {
		}

		public void OnPost() {
		}
	}
}
