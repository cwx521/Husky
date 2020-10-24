using System.Linq;
using System.Threading.Tasks;
using Husky.Principal.Users.Data;
using Husky.TwoFactor;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal.Users
{
	public partial class UserProfileManager
	{
		public async Task<Result> UseNewPassword(string newPassword) {
			if ( _me.IsAnonymous ) {
				return new Failure("需要先登录");
			}

			var oldPasswords = _db.UserPasswords.Where(x => !x.IsObsolete).Where(x => x.UserId == _me.Id).ToList();
			oldPasswords.ForEach(x => x.IsObsolete = true);

			_db.UserPasswords.Add(new UserPassword {
				UserId = _me.Id,
				Password = Crypto.SHA1(newPassword)
			});

			await _db.Normalize().SaveChangesAsync();
			return new Success();
		}

		public async Task<Result> UseNewPasswordWithPhoneValidation(string newPassword, string verificationCode) {
			var userPhone = _db.UserPhones.Find(_me.Id);
			if ( userPhone == null ) {
				return new Failure("需要先绑定手机");
			}

			var twoFactor = _me.ServiceProvider.GetRequiredService<ITwoFactorManager>();
			var verifyResult = await twoFactor.VerifyCodeAsync(userPhone.Number, verificationCode, true);

			if ( !verifyResult.Ok ) {
				return verifyResult;
			}
			userPhone.IsVerified = true;

			return await UseNewPassword(newPassword);
		}
	}
}
