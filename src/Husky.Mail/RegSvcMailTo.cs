using Husky.Data;
using Husky.Mail;
using Husky.Mail.Abstractions;
using Husky.Mail.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Injection
{
	public static class RegSvcMailTo
	{
		public static IServiceCollection AddHuskyMailToPlugin(this IServiceCollection services, string dbConnectionString = null) {
			services.AddDbContext<MailDbContext>((svc, builder) => builder.UseSqlServer(dbConnectionString ?? svc.GetRequiredService<IConfiguration>().FindConnectionStringBySequence<MailDbContext>()));
			services.AddSingleton<IMailSender>(svc => new MailSender(svc));
			return services;
		}
	}
}