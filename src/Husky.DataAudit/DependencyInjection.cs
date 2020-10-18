using System;
using Husky.DataAudit.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyInjector AddDataAudit(this HuskyInjector husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services.AddDbContextPool<AuditDbContext>(optionsAction);
			return husky;
		}
	}
}