using System.Threading.Tasks;
using Husky.Principal.Users.Data;

namespace Husky.Principal.Users
{
	public partial class UserGroupsManager
	{
		public async Task<Result> JoinGroupAsync(int groupId) {
			if ( _me.IsAnonymous ) {
				return new Failure("需要先登录");
			}

			_db.Normalize().AddOrUpdate(new UserInGroup {
				UserId = _me.Id,
				GroupId = groupId
			});

			await _db.Normalize().SaveChangesAsync();
			return new Success();
		}

		public async Task<Result> JoinGroupAsync(UserGroup userGroup) => await JoinGroupAsync(userGroup.Id);
	}
}