using Husky.Mail;
using Husky.Mail.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddDiagnostics(this HuskyDI husky, string nameOfConnectionString = null, bool migrateRequiredDatabase = true) {
			husky.Services
				.AddDbContextPool<MailDbContext>(nameOfConnectionString, migrateRequiredDatabase)
				.AddScoped<IMailSender, MailSender>();

			return husky;
		}
	}
}
