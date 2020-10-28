using System.Linq;
using Husky;
using Husky.Principal.Users.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal.Users
{
	public static partial class PrincipalExtensions
	{
		public static UserQuickViewModel QuickView(this IPrincipalUser principal) {
			if ( principal.Id == 0 ) {
				return new UserQuickViewModel();
			}

			return (UserQuickViewModel)principal.CacheData().GetOrAdd(nameof(UserQuickViewModel), key => {
				using var scope = principal.ServiceProvider.CreateScope();
				var db = scope.ServiceProvider.GetRequiredService<IUsersDbContext>();

				var quickView = db.Users
					.AsNoTracking()
					.Where(x => x.Id == principal.Id)
					.Select(x => new UserQuickViewModel {
						PhotoUrl = x.PhotoUrl ?? (x.WeChat == null ? null : x.WeChat.HeadImageUrl),
						PhoneNumber = x.Phone == null ? null : x.Phone.Number,
						EmailAddress = x.Email == null ? null : x.Email.EmailAddress,
						RegisteredTime = x.RegisteredTime,
						AwaitChangePassword = Todo.NoNeed
					})
					.SingleOrDefault();

				if ( quickView == null ) {
					principal.Auth().SignOut();
				}
				return quickView ?? new UserQuickViewModel();
			});
		}

		public static UserAuthManager Auth(this IPrincipalUser principal) => new UserAuthManager(principal);
		public static UserProfileManager Profile(this IPrincipalUser principal) => new UserProfileManager(principal);
		public static UserGroupsManager Groups(this IPrincipalUser principal) => new UserGroupsManager(principal);
	}
}
