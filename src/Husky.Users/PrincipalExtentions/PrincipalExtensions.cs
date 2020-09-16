using System.Linq;
using Husky;
using Husky.Users.Data;
using Husky.Principal.SessionData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal
{
	public static partial class PrincipalExtensions
	{
		public static AuthManager UserAuth(this IPrincipalUser principal) {
			return new AuthManager(principal);
		}

		public static UserQuickView QuickView(this IPrincipalUser principal) {
			var sessionData = principal.SessionData();
			if ( sessionData == null ) {
				return new UserQuickView();
			}
			return (UserQuickView)sessionData.GetOrAdd(nameof(UserQuickView), key => {
				if ( principal.Id == 0 ) {
					principal.UserAuth().SignOut();
					return new UserQuickView();
				}

				using var scope = principal.ServiceProvider.CreateScope();
				var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

				var quickView = db.Users
					.AsNoTracking()
					.Where(x => x.Id == principal.Id)
					.Select(x => new UserQuickView {
						PhotoUrl = x.PhotoUrl ?? (x.WeChat == null ? null : x.WeChat.HeadImageUrl),
						PhoneNumber = x.Phone == null ? null : x.Phone.Number,
						RegisteredTime = x.RegisteredTime,
						AwaitChangePassword = ActionAwait.NoNeed
					})
					.SingleOrDefault();

				if ( quickView == null ) {
					principal.UserAuth().SignOut();
				}
				return quickView ?? new UserQuickView();
			});
		}
	}
}
