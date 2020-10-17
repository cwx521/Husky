using System;
using Husky.Mail;
using Husky.Mail.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddMailSender(this HuskyDI husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services
				.AddDbContextPool<MailDbContext>(optionsAction)
				.AddScoped<IMailDbContext, MailDbContext>()
				.AddScoped<IMailSender, MailSender>();
			return husky;
		}

		public static HuskyDI AddMailSender<TDbContext>(this HuskyDI husky)
			where TDbContext : DbContext, IMailDbContext {
			husky.Services
				.AddScoped<IMailDbContext, TDbContext>()
				.AddScoped<IMailSender, MailSender>();
			return husky;
		}

		public static HuskyDI MapMailSender<TMailSenderImplement>(this HuskyDI husky)
			where TMailSenderImplement : class, IMailSender {
			husky.Services.AddScoped<IMailSender, TMailSenderImplement>();
			return husky;
		}

		public static HuskyDI MapMailSender<TMailSenderImplement>(this HuskyDI husky, Func<IServiceProvider, TMailSenderImplement> implementationFactory)
			where TMailSenderImplement : class, IMailSender {
			husky.Services.AddScoped<IMailSender, TMailSenderImplement>(implementationFactory);
			return husky;
		}
	}
}
