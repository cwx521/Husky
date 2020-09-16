using Husky.DataAudit.Data;
using Husky.Diagnostics.Data;
using Husky.KeyValues.Data;
using Husky.Mail.Data;
using Husky.Payments.Data;
using Husky.TwoFactor.Data;
using Husky.Users.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Tests
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services) {
			var connstr = "Data Source=.; Initial Catalog=HuskyTest; Integrated Security=True";

			services.AddDbContextPool<AuditDbContext>(x => x.UseSqlServer(connstr));
			services.AddDbContextPool<DiagnosticsDbContext>(x => x.UseSqlServer(connstr));
			services.AddDbContextPool<MailDbContext>(x => x.UseSqlServer(connstr));
			services.AddDbContextPool<KeyValueDbContext>(x => x.UseSqlServer(connstr));
			services.AddDbContextPool<TwoFactorDbContext>(x => x.UseSqlServer(connstr));

			services.AddDbContextPool<UsersDbContext>(x => x.UseSqlServer(connstr));
			services.AddDbContextPool<PaymentsDbContext>(x => x.UseSqlServer(connstr));

			/*
			add-migration  Init_DataAudit  -context AuditDbContext -project Husky.DataAudit -o Data/Migrations
			add-migration  Init_KeyValue  -context KeyValueDbContext -project Husky.KeyValues -o Data/Migrations
			add-migration  Init_Mail  -context MailDbContext -project Husky.Mail -o Data/Migrations
			add-migration  Init_Diagnostics  -context DiagnosticsDbContext -project Husky.Diagnostics -o Data/Migrations
			add-migration  Init_TwoFactor  -context TwoFactorDbContext -project Husky.TwoFactor -o Data/Migrations
			
			add-migration  Init_Users  -context UsersDbContext -project Husky.Users -o Data/Migrations
			add-migration  Init_Payments  -context PaymentsDbContext -project Husky.Payments -o Data/Migrations
			*/
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
		}
	}
}
