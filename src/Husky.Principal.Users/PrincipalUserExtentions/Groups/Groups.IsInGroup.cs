using System.Linq;
using Husky.Principal.Users.Data;

namespace Husky.Principal.Users
{
	public partial class UserGroupsFunctions
	{
		public bool IsInGroup(int groupId) => _me.IsAuthenticated && GetGroups().Any(x => x.Id == groupId);
		public bool IsInGroup(UserGroup userGroup) => IsInGroup(userGroup.Id);
	}
}