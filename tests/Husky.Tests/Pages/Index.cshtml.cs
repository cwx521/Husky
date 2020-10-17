using System.Linq;
using Husky.Principal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Husky.Tests.Pages
{
	public class IndexPageModel : PageModel
	{
		public IndexPageModel(IPrincipalUser principal) {
			_me = principal;
		}

		private IPrincipalUser _me;

		public string TellHim { get; private set; }

		public void OnGet() {
			if ( _me.IsAnonymous ) {
				_me.Id = 1;
				_me.DisplayName = "Weixing";
				_me.IdentityManager.SaveIdentity(_me);
			}
		}

		public IActionResult OnPost() {
			if ( !ModelState.IsValid ) {
				TellHim = ModelState.GetAllErrorMessages().First();
			}
			return Page();
		}
	}
}
