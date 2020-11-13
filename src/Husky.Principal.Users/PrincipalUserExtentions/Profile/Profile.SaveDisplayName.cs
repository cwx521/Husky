using System.Linq;
using System.Threading.Tasks;
using Husky.Principal.Users.Data;

namespace Husky.Principal.Users
{
	public partial class UserProfileFunctions
	{
		public async Task<Result> SaveDisplayNameAsync(string displayName, bool allowDuplication = false) {
			if ( _me.IsAnonymous ) {
				return new Failure("需要先登录");
			}

			if ( displayName.Length > 18 ) {
				return new Failure("不能超过18个字符");
			}
			if ( !allowDuplication ) {
				var isTaken = _db.Users.Any(x => x.Id != _me.Id && x.DisplayName == displayName);
				if ( isTaken ) {
					return new Failure("此名字已经被别人占用");
				}
			}

			var user = new User { Id = _me.Id };
			_db.Users.Attach(user);
			user.DisplayName = displayName;

			await _db.Normalize().SaveChangesAsync();
			return new Success();
		}
	}
}