using System;
using Husky.Principal;
using Husky.Principal.Implements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddAdminContext(this HuskyDI husky) {
			var key = typeof(IPrincipalUser).FullName;

			husky.Services
				.AddScoped<IAdminContext, AdminContext>();

			return husky;
		}
	}
}
