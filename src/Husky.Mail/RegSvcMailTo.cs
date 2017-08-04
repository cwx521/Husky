using Husky.Data.Abstractions;
using Husky.Mail.Abstractions;
using Husky.Mail.Data;
using Husky.Mail;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Injection
{
	public static class RegSvcMailTo
	{
		public static IServiceCollection AddHuskyMailToPlugin(this IServiceCollection services, IDatabaseFinder database = null) => services
			.AddScoped(svc => new MailDbContext(database ?? svc.GetService<IDatabaseFinder>()))
			.AddSingleton<IMailSender>(svc => new MailSender(svc));
	}
}