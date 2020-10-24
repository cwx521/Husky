using System.Linq;
using Husky.Principal.Users.Data;

namespace Husky.Principal.Users
{
	public partial class UserGroupsManager
	{
		public bool IsInGroup(int groupId) {
			return _me.IsAuthenticated &&
				   _db.UserInGroups.Any(x => x.UserId == _me.Id && x.GroupId == groupId);
		}

		public bool IsInGroup(UserGroup userGroup) => IsInGroup(userGroup.Id);
	}
}