using System;
using System.Collections.Generic;
using Husky.Authentication;
using Husky.Authentication.Implementations;
using Husky.Users;
using Husky.Users.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

		private string GetConnectionString() => Configuration.GetConnectionString("Default");

		public void ConfigureServices(IServiceCollection services) {
			services.AddMvc();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddSingleton<IConfiguration>(Configuration);

			services.AddHuskyPrincipal(IdentityCarrier.Cookie, new IdentityOptions { Token = Configuration.GetValue<string>("SecretToken") });
			services.AddHuskyUserModule(db => db.UseSqlServer(GetConnectionString()));
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
			if ( env.IsDevelopment() ) {
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}

			app.UseStaticFiles();
			app.UseMvc(routes => routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}"));

			new List<Type> {
				typeof(UserDbContext)
			}.
			ForEach(x => (app.ApplicationServices.GetRequiredService(x) as DbContext).Database.EnsureCreated());
		}
	}
}
