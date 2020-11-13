using System;
using System.Linq;
using Husky.Principal.Users.Data;
using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.Users
{
	public partial class UserGroupsFunctions
	{
		private const string _groupsCacheKey = nameof(GetGroups);

		public UserGroup[] GetGroups() {
			if ( _me.IsAnonymous ) {
				return Array.Empty<UserGroup>();
			}

			return (UserGroup[])_me.Cache().GetOrAdd(_groupsCacheKey, key => _db.UserInGroups
				.AsNoTracking()
				.Where(x => x.UserId == _me.Id)
				.Select(x => x.Group)
				.ToArray()
			);
		}
	}
}