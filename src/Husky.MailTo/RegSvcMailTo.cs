using Husky.Data.Abstractions;
using Husky.MailTo.Abstractions;
using Husky.MailTo.Data;
using Husky.Smtp;
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