using System.Threading.Tasks;
using Husky.Principal.Users.Data;

namespace Husky.Principal.Users
{
	public partial class UserProfileFunctions
	{
		public async Task<Result> SaveRealAsync(string socialIdNumber, string realName, bool isVerified = false) {
			if ( _me.IsAnonymous ) {
				return new Failure("需要先登录");
			}
			if ( socialIdNumber.IsMainlandSocialNumber() ) {
				return new Failure("身份证号码不正确");
			}

			var userReal = await _db.UserReals.FindAsync(_me.Id);
			if ( userReal == null ) {
				userReal = new UserReal {
					UserId = _me.Id
				};
				_db.UserReals.Add(userReal);
			}

			userReal.SocialIdNumber = socialIdNumber;
			userReal.RealName = realName;
			userReal.Sex = StringTest.GetSexFromMainlandSocialNumber(socialIdNumber);
			userReal.IsVerified = isVerified;

			await _db.Normalize().SaveChangesAsync();
			return new Success();
		}
	}
}