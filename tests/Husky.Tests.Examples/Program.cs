using Husky;
using Husky.Diagnostics;
using Husky.Diagnostics.Data;
using Husky.FileStore.Data;
using Husky.KeyValues.Data;
using Husky.Mail.Data;
using Husky.Principal.Administration.Data;
using Husky.Principal.AntiViolence;
using Husky.Principal.UserMessages.Data;
using Husky.Principal.Users.Data;
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
builder.Services.AddDbContextPool<DiagnosticsDbContext>(x => x.UseSqlServer(connstr).Migrate());
builder.Services.AddDbContextPool<KeyValueDbContext>(x => x.UseSqlServer(connstr).Migrate());
builder.Services.AddDbContextPool<FileStoreDbContext>(x => x.UseSqlServer(connstr).Migrate());
builder.Services.AddDbContextPool<TwoFactorDbContext>(x => x.UseSqlServer(connstr));
builder.Services.AddDbContextPool<AdminsDbContext>(x => x.UseSqlServer(connstr));
builder.Services.AddDbContextPool<UsersDbContext>(x => x.UseSqlServer(connstr));
builder.Services.AddDbContextPool<UserMessagesDbContext>(x => x.UseSqlServer(connstr));
builder.Services.AddDbContextPool<MailDbContext>(x => x.UseSqlServer(connstr));

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
husky.AddPrincipal();
husky.AddDiagnostics<DiagnosticsDbContext>();
husky.AddKeyValueManager<KeyValueDbContext>();


/*
add-migration  Init_KeyValue  -context KeyValueDbContext -project Husky.KeyValues -o Data/Migrations
add-migration  Init_FileStore  -context FileStoreDbContext -project Husky.FileStore -o Data/Migrations
add-migration  Init_Mail  -context MailDbContext -project Husky.Mail -o Data/Migrations
add-migration  Init_Diagnostics  -context DiagnosticsDbContext -project Husky.Diagnostics -o Data/Migrations
add-migration  Init_TwoFactor  -context TwoFactorDbContext -project Husky.TwoFactor -o Data/Migrations
add-migration  Init_Admins  -context AdminsDbContext -project Husky.Principal.Administration -o Data/Migrations
add-migration  Init_Users  -context UsersDbContext -project Husky.Principal.Users -o Data/Migrations
add-migration  Init_UserMessages  -context UserMessagesDbContext -project Husky.Principal.UserMessages -o Data/Migrations
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