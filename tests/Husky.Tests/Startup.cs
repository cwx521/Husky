using Husky.Diagnostics;
using Husky.Diagnostics.Data;
using Husky.KeyValues.Data;
using Husky.Mail.Data;
using Husky.Principal.Administration.Data;
using Husky.Principal.AntiViolence;
using Husky.Principal.UserMessages.Data;
using Husky.Principal.Users.Data;
using Husky.TwoFactor.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Tests
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services) {
			Crypto.PermanentToken = "DevTest";

			//(localdb)\MSSQLLocalDB
			var connstr = @"Data Source=.; Initial Catalog=HuskyTest; Integrated Security=True";
			services.AddDbContextPool<DiagnosticsDbContext>(x => x.UseSqlServer(connstr).Migrate());
			services.AddDbContextPool<KeyValueDbContext>(x => x.UseSqlServer(connstr).Migrate());
			services.AddDbContextPool<MailDbContext>(x => x.UseSqlServer(connstr));
			services.AddDbContextPool<TwoFactorDbContext>(x => x.UseSqlServer(connstr));
			services.AddDbContextPool<AdminsDbContext>(x => x.UseSqlServer(connstr));
			services.AddDbContextPool<UsersDbContext>(x => x.UseSqlServer(connstr));
			services.AddDbContextPool<UserMessagesDbContext>(x => x.UseSqlServer(connstr));

			//AspNet
			services.AddRazorPages().AddMvcOptions(mvc => {
				mvc.Filters.Add<ExceptionLogHandlerFilter>();
				mvc.Filters.Add<RequestLogHandlerFilter>();
				mvc.Filters.Add<AntiViolenceFilter>();
			});
			services.AddSession();
			services.AddSingleton<IMemoryCache, MemoryCache>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.Husky()
				.AddPrincipal()
				.AddDiagnostics<DiagnosticsDbContext>()
				.AddKeyValueManager<KeyValueDbContext>();

			/*
			add-migration  Init_DataAudit  -context AuditDbContext -project Husky.DataAudit -o Data/Migrations
			add-migration  Init_KeyValue  -context KeyValueDbContext -project Husky.KeyValues -o Data/Migrations
			add-migration  Init_Mail  -context MailDbContext -project Husky.Mail -o Data/Migrations
			add-migration  Init_Diagnostics  -context DiagnosticsDbContext -project Husky.Diagnostics -o Data/Migrations
			add-migration  Init_TwoFactor  -context TwoFactorDbContext -project Husky.TwoFactor -o Data/Migrations
			add-migration  Init_Admins  -context AdminsDbContext -project Husky.Principal.Administration -o Data/Migrations
			add-migration  Init_Users  -context UsersDbContext -project Husky.Principal.Users -o Data/Migrations
			add-migration  Init_UserMessages  -context UserMessagesDbContext -project Husky.Principal.UserMessages -o Data/Migrations
			*/
		}

		public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			app.UseDeveloperExceptionPage();
			app.UseHttpsRedirection();
			app.UseSession();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseAuthorization();
			app.UseEndpoints(endpoints => {
				endpoints.MapRazorPages();
			});
		}
	}
}
