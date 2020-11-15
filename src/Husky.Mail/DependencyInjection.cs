using System;
using Husky.Mail;
using Husky.Mail.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyServiceHub AddMailSender(this HuskyServiceHub husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services.AddDbContextPool<IMailDbContext, MailDbContext>(optionsAction);
			husky.Services.AddScoped<IMailSender, MailSender>();
			return husky;
		}

		public static HuskyServiceHub AddMailSender<TDbContext>(this HuskyServiceHub husky)
			where TDbContext : DbContext, IMailDbContext {
			husky.Services.AddDbContext<IMailDbContext, TDbContext>();
			husky.Services.AddScoped<IMailSender, MailSender>();
			return husky;
		}
	}
}
