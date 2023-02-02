using System;
using System.Collections.Generic;
using System.Linq;
using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Husky.Tests.Examples.Pages
{
	public class IndexPageModel : PageModel
	{
		public IndexPageModel(IPrincipalUser principal, IDiagnosticsDbContext db, Config config) {
			_me = principal;
			_db = db;
			_config = config;
		}

		private readonly IPrincipalUser _me;
		private readonly IDiagnosticsDbContext _db;
		private readonly Config _config;

		public string? TellHim { get; private set; }
		public List<RequestLog> RequestLogs { get; private set; } = null!;

		public void OnGet() {
			RequestLogs = _db.RequestLogs.AsNoTracking().ToList();
		}

		public IActionResult OnPost() {
			if (!ModelState.IsValid) {
				TellHim = ModelState.GetAllErrorMessages().First();
			}
			if (_me.IsAnonymous) {
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
