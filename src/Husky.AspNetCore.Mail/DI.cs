using Husky.AspNetCore.Mail;
using Husky.AspNetCore.Mail.Abstractions;
using Husky.AspNetCore.Mail.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.AspNetCore
{
	public static class DI
	{
		public static HuskyDependencyInjectionHub AddMail(this HuskyDependencyInjectionHub husky, string dbConnectionString = null) {
			husky.ServiceCollection
				.AddDbContext<MailDbContext>((svc, builder) => {
					builder.UseSqlServer(
						dbConnectionString ??
						svc.GetRequiredService<IConfiguration>().GetConnectionStringBySeekSequence<MailDbContext>()
					);
					builder.Migrate();
				})
				.AddSingleton<IMailSender, MailSender>();

			return husky;
		}
	}
}
