using Husky.Mail;
using Husky.Mail.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.DependencyInjection
{
	public static class DependencyInjection
	{
		private static bool migrated = false;

		public static HuskyDependencyInjectionHub AddMail(this HuskyDependencyInjectionHub husky, string nameOfConnectionString = null) {
			husky.Services
				.AddDbContextPool<MailDbContext>((svc, builder) => {
					var config = svc.GetRequiredService<IConfiguration>();
					var connstr = config.SeekConnectionString<MailDbContext>(nameOfConnectionString);
					builder.UseSqlServer(connstr);

					if ( !migrated ) {
						builder.Migrate();
						migrated = true;
					}
				})
				.AddSingleton<IMailSender, MailSender>();

			return husky;
		}
	}
}
