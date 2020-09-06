using System;
using Husky.CommonModules.Users.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddUserModule(this HuskyDI husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services.AddDbContextPool<UserModuleDbContext>(optionsAction);
			return husky;
		}
	}
}
