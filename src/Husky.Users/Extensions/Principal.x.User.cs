using Husky.Authentication.Abstractions;
using Husky.Users.Data;
using Microsoft.AspNetCore.Http;

namespace Husky.Users.Extensions
{
	public sealed partial class PrincipalUserExtensions
	{
		public PrincipalUserExtensions(IPrincipal principal, IHttpContextAccessor httpContextAccessor, UserDbContext userDb) {
			_my = principal;
			_http = httpContextAccessor.HttpContext;
			_userDb = userDb;
		}

		readonly IPrincipal _my;
		readonly HttpContext _http;
		readonly UserDbContext _userDb;
	}
}
