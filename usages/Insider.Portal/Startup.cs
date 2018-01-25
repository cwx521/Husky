using Husky.Authentication;
using Husky.Authentication.Implements;
using Husky.Injection;
using Husky.Sugar;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Insider.Portal
{
	public class Startup
	{
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services) {
			Crypto.PermanentToken = Configuration.GetValue<string>("AppVariables:PermanentToken");

			services.AddMvc();
			services.AddSingleton(Configuration);
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddHuskyAuthentication(IdType.Guid, IdentityCarrier.Cookie);
			services.AddHuskyUsersPlugin();
			services.AddHuskyMailPlugin();
			services.AddHuskyTwoFactorPlugin();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
			if ( env.IsDevelopment() ) {
				app.UseDeveloperExceptionPage();
			}

			app.UseStaticFiles();
			app.UseMvc(routes => routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}"));

			//using ( var scope = app.ApplicationServices.CreateScope() ) {
			//	var mailDb = scope.ServiceProvider.GetRequiredService<MailDbContext>();
			//	if ( !mailDb.MailSmtpProviders.Any() ) {
			//		mailDb.Add(new MailSmtpProvider {
			//			Host = "smtp.live.com",
			//			Port = 587,
			//			Ssl = false,
			//			CredentialName = "chenwx521@hotmail.com",
			//			Password = "",
			//			SenderDisplayName = "Weixing Chen",
			//			SenderMailAddress = "chenwx521@hotmail.com",
			//			IsInUse = true
			//		});
			//		mailDb.SaveChanges();
			//	}
			//}
		}
	}
}
