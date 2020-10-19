using System;
using Husky.Mail;
using Husky.Mail.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyInjector AddMailSender(this HuskyInjector husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services
				.AddDbContextPool<MailDbContext>(optionsAction)
				.AddScoped<IMailDbContext, MailDbContext>()
				.AddScoped<IMailSender, MailSender>();
			return husky;
		}

		public static HuskyInjector AddMailSender<TDbContext>(this HuskyInjector husky)
			where TDbContext : DbContext, IMailDbContext {
			husky.Services
				.AddScoped<IMailDbContext, TDbContext>()
				.AddScoped<IMailSender, MailSender>();
			return husky;
		}
	}
}
