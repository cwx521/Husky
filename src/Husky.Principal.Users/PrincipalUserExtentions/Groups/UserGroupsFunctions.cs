using Husky.Principal.Users.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal.Users
{
	public sealed partial class UserGroupsFunctions
	{
		internal UserGroupsFunctions(IPrincipalUser principal) {
			_me = principal;
			_db = principal.ServiceProvider.GetRequiredService<IUsersDbContext>();
		}

		private readonly IPrincipalUser _me;
		private readonly IUsersDbContext _db;
	}
}
