using Husky.Authentication;
using Husky.Authentication.Implements;
using Husky.Data;
using Husky.Data.Abstractions;
using Husky.Injection;
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
		public Startup(IHostingEnvironment env) {
			Configuration = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile($"Config/appSettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"Config/appSettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();
		}

		public IConfigurationRoot Configuration { get; }

		public void ConfigureServices(IServiceCollection services) {
			services.AddMvc();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddSingleton<IConfiguration>(Configuration);

			services.AddSingleton<IDatabaseFinder>(new DatabaseFinder(DatabaseProvider.SqlServer, Configuration.GetConnectionString("Default")));
			services.AddHuskyAuthentication(IdType.Guid, IdentityCarrier.Cookie, new IdentityOptions { Token = Configuration.GetValue<string>("SecretToken") });
			services.AddHuskyUsersPlugin();
			services.AddHuskyMailToPlugin();
			services.AddHuskyTwoFactorPlugin();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
			if ( env.IsDevelopment() ) {
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}

			app.UseStaticFiles();
			app.UseMvc(routes => routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}"));

			//new List<Type> {
			//	typeof(UserDbContext),
			//	typeof(MailDbContext),
			//	typeof(TwoFactorDbContext),
			//}.
			//ForEach(async x => {
			//	var database = (app.ApplicationServices.GetRequiredService(x) as DbContext).Database;
			//	await database.MigrateAsync();
			//});
		}
	}
}
