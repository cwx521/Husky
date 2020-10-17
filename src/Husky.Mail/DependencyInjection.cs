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

		public static HuskyDI AddMailSender<TMailDbContextImplement>(this HuskyDI husky)
			where TMailDbContextImplement : class, IMailDbContext {
			husky.Services
				.AddScoped<IMailDbContext, TMailDbContextImplement>()
				.AddScoped<IMailSender, MailSender>();
			return husky;
		}

		public static HuskyDI AddMailSenderWithOwnImplement<TMailSenderImplement>(this HuskyDI husky)
			where TMailSenderImplement : class, IMailSender {
			husky.Services.AddScoped<IMailSender, TMailSenderImplement>();
			return husky;
		}
	}
}
