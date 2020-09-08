using Husky.CommonModules.Users.Data;
using Husky.WeChatIntegration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal
{
	public sealed partial class AuthManager
	{
		internal AuthManager(IPrincipalUser principal) {
			_me = principal;
			_db = principal.ServiceProvider.GetRequiredService<UserModuleDbContext>();
			_http = principal.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
			_wechat = principal.ServiceProvider.GetService<WeChatIntegrationManager>();
		}

		private readonly IPrincipalUser _me;
		private readonly UserModuleDbContext _db;
		private readonly HttpContext _http;
		private readonly WeChatIntegrationManager? _wechat;		
	}
}
