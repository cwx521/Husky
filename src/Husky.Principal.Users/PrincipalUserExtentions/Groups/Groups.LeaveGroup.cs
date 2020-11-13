using System.Threading.Tasks;
using Husky.Principal.Users.Data;

namespace Husky.Principal.Users
{
	public partial class UserGroupsFunctions
	{
		public async Task<Result> LeaveGroupAsync(int groupId) {
			if ( _me.IsAnonymous ) {
				return new Failure("需要先登录");
			}

			var row = _db.UserInGroups.Find(_me.Id, groupId);
			if ( row != null ) {
				_db.UserInGroups.Remove(row);
			}

			await _db.Normalize().SaveChangesAsync();
			_me.Cache().TryRemove(_groupsCacheKey, out _);
			return new Success();
		}

		public async Task<Result> LeaveGroupAsync(UserGroup userGroup) => await LeaveGroupAsync(userGroup.Id);
	}
}