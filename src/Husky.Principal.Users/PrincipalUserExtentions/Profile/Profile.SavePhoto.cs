using System.Threading.Tasks;
using Husky.Principal.Users.Data;

namespace Husky.Principal.Users
{
	public partial class UserProfileFunctions
	{
		public async Task<Result> SavePhotoAsync(string photoUrl) {
			if ( _me.IsAnonymous ) {
				return new Failure("需要先登录");
			}

			var user = new User { Id = _me.Id };
			_db.Users.Attach(user);
			user.PhotoUrl = photoUrl;

			await _db.Normalize().SaveChangesAsync();
			return new Success();
		}
	}
}