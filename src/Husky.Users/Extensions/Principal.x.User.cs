using System;
using Husky.Authentication;
using Husky.Authentication.Abstractions;
using Husky.Users.Data;
using Microsoft.AspNetCore.Http;

namespace Husky.Users.Extensions
{
	public sealed partial class PrincipalUserExtensions
	{
		public PrincipalUserExtensions(Principal<Guid> principal, IHttpContextAccessor httpContextAccessor, UserDbContext userDb) {
			_my = principal;
			_http = httpContextAccessor.HttpContext;
			_userDb = userDb;
		}

		readonly Principal<Guid> _my;
		readonly HttpContext _http;
		readonly UserDbContext _userDb;
	}
}
