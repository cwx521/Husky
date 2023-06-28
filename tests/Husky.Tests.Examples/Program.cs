using Husky;
using Husky.Diagnostics;
using Husky.Diagnostics.Data;
using Husky.FileStore.Data;
using Husky.KeyValues.Data;
using Husky.Mail.Data;
using Husky.Principal.AntiViolence;
using Husky.Tests.Examples;
using Husky.TwoFactor.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var connstr = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=HuskyTest; Integrated Security=True";
Crypto.SecretToken = "DevTest";

//(localdb)\MSSQLLocalDB
builder.Services.AddDbContextPool<DiagnosticsDbContext>(x => x.UseSqlServer(connstr));
builder.Services.AddDbContextPool<KeyValueDbContext>(x => x.UseSqlServer(connstr));
builder.Services.AddDbContextPool<FileStoreDbContext>(x => x.UseSqlServer(connstr));
builder.Services.AddDbContextPool<TwoFactorDbContext>(x => x.UseSqlServer(connstr));
builder.Services.AddDbContextPool<MailDbContext>(x => x.UseSqlServer(connstr));
builder.Services.AddDbContextPool<NotificationTaskDbContext>(x => x.UseSqlServer(connstr));

//AspNet
builder.Services.AddRazorPages().AddMvcOptions(mvc => {
	mvc.Filters.Add<ExceptionLogHandlerFilter>();
	mvc.Filters.Add<RequestLogHandlerFilter>();
	mvc.Filters.Add<AntiViolenceFilter>();
});
builder.Services.AddSession();
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<Config>();

var husky = builder.Services.Husky();
husky.AddPrincipal(x => x.Carrier = IdentityCarrier.Cookie);
husky.AddDiagnostics<DiagnosticsDbContext>();
husky.AddKeyValueManager<KeyValueDbContext>();
husky.AddNotificationTasks<NotificationTaskDbContext>();


/*
add-migration  KeyValue_Init  -context KeyValueDbContext -project Husky.KeyValues -o Data/Migrations
add-migration  FileStore_Init  -context FileStoreDbContext -project Husky.FileStore -o Data/Migrations
add-migration  Mail_Init  -context MailDbContext -project Husky.Mail -o Data/Migrations
add-migration  Diagnostics_Init  -context DiagnosticsDbContext -project Husky.Diagnostics -o Data/Migrations
add-migration  TwoFactor_Init  -context TwoFactorDbContext -project Husky.TwoFactor -o Data/Migrations
add-migration  NotificationTasks_Init  -context NotificationTaskDbContext -project Husky.NotificationTasks -o Data/Migrations
*/


var app = builder.Build();
app.UseDeveloperExceptionPage();
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapDefaultControllerRoute();
app.MapRazorPages();
app.Run();