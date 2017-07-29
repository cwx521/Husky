using Husky.MailTo.Abstractions;
using Husky.MailTo.Data;
using Husky.Smtp;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.MailTo
{
	public static class ServiceRegisterMailTo
	{
		public static IServiceCollection AddHuskyMailToPlugin(this IServiceCollection services) => services
			.AddDbContext<MailDbContext>()
			.AddSingleton<IMailSender>(serviceProvider => new MailSender(serviceProvider));
	}
}