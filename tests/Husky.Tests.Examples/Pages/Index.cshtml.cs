using System.Collections.Generic;
using System.Linq;
using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Husky.Tests.Examples.Pages
{
	public class IndexPageModel : PageModel
	{
		public IndexPageModel(IPrincipalUser principal, IDiagnosticsDbContext db) {
			_me = principal;
			_db = db;
		}

		private readonly IPrincipalUser _me;
		private readonly IDiagnosticsDbContext _db;

		public string? TellHim { get; private set; }
		public List<RequestLog> RequestLogs { get; private set; } = null!;

		public void OnGet() {
			RequestLogs = _db.RequestLogs.AsNoTracking().ToList();
		}

		public IActionResult OnPost() {
			if ( !ModelState.IsValid ) {
				TellHim = ModelState.GetAllErrorMessages().First();
			}
			if ( _me.IsAnonymous ) {
				_me.Id = 1;
				_me.DisplayName = "Weixing";
				_me.IdentityManager!.SaveIdentity(_me);
				return Redirect("/");
			}
			OnGet();
			return Page();
		}
	}
}
