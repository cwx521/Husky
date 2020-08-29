using Husky.Mail;
using Husky.Mail.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		private static bool migrated = false;

		public static HuskyDI AddMail(this HuskyDI husky, string nameOfConnectionString = null) {
			husky.Services
				.AddDbContextPool<MailDbContext>((svc, builder) => {
					var config = svc.GetRequiredService<IConfiguration>();
					var connstr = config.SeekConnectionString<MailDbContext>(nameOfConnectionString);
					builder.UseSqlServer(connstr);

					if ( !migrated ) {
						builder.CreateDbContext().Database.Migrate();
						migrated = true;
					}
				})
				.AddSingleton<IMailSender, MailSender>();

			return husky;
		}
	}
}
