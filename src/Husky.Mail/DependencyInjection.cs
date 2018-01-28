using Husky.Mail;
using Husky.Mail.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.DependencyInjection
{
	public static class DependencyInjection
	{
		public static HuskyDependencyInjectionHub AddMail(this HuskyDependencyInjectionHub husky, string nameOfConnectionString = null) {
			husky.Services
				.AddDbContext<MailDbContext>((svc, builder) => {
					var config = svc.GetRequiredService<IConfiguration>();
					var connstr = config.SeekConnectionStringSequence<MailDbContext>(nameOfConnectionString);
					builder.UseSqlServer(connstr);
					builder.Migrate();
				})
				.AddSingleton<IMailSender, MailSender>();

			return husky;
		}
	}
}
