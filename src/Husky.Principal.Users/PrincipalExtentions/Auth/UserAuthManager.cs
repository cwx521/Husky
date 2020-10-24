using Husky.Principal.Users.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal.Users
{
	public sealed partial class UserAuthManager
	{
		internal UserAuthManager(IPrincipalUser principal) {
			_me = principal;
			_db = principal.ServiceProvider.GetRequiredService<IUsersDbContext>();
			_http = principal.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
		}

		private readonly IPrincipalUser _me;
		private readonly IUsersDbContext _db;
		private readonly HttpContext _http;
	}
}
