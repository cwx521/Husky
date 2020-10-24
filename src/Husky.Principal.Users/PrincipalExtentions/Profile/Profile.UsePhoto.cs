using System.Threading.Tasks;

namespace Husky.Principal.Users
{
	public partial class UserProfileManager
	{
		public async Task<Result> UsePhoto(string photoUrl) {
			if ( _me.IsAnonymous ) {
				return new Failure("需要先登录");
			}

			var user = _db.Users.Find(_me.Id);
			user.PhotoUrl = photoUrl;

			await _db.Normalize().SaveChangesAsync();
			return new Success();
		}
	}
}