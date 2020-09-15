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
				.AddScoped<IMailSender, MailSender>();

			return husky;
		}
	}
}
