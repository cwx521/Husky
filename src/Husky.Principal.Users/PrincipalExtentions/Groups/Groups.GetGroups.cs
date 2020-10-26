using System.Linq;
using Husky.Principal.Users.Data;
using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.Users
{
	public partial class UserGroupsManager
	{
		private static string _groupsCacheKey = nameof(GetGroups);

		public UserGroup[] GetGroups() {
			if ( _me.IsAnonymous ) {
				return new UserGroup[0];
			}

			return (UserGroup[])_me.CacheData().GetOrAdd(_groupsCacheKey, key => _db.UserInGroups
				.AsNoTracking()
				.Where(x => x.UserId == _me.Id)
				.Select(x => x.Group)
				.ToArray()
			);
		}
	}
}