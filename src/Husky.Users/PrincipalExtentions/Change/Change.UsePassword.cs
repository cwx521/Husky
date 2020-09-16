using System.Linq;
using System.Threading.Tasks;
using Husky.TwoFactor;
using Husky.Users.Data;

namespace Husky.Principal
{
	partial class ChangeManager
	{
		public async Task<Result> UsePassword(string newPassword, string verificationCode) {
			if ( _me.IsAnonymous ) {
				return new Failure("需要先登录");
			}

			var userPhone = _db.UserPhones.Find(_me.Id);
			if ( userPhone == null ) {
				return new Failure("需要先绑定手机才能设置密码");
			}

			var validationModel = new TwoFactorModel { SendTo = userPhone.Number, Code = verificationCode };
			var validationResult = await _me.TwoFactor().VerifyTwoFactorCode(validationModel, true);

			if ( !validationResult.Ok ) {
				return validationResult;
			}
			userPhone.IsVerified = true;

			var oldPasswords = _db.UserPasswords.Where(x => !x.IsObsoleted).Where(x => x.UserId == _me.Id).ToList();
			oldPasswords.ForEach(x => x.IsObsoleted = true);

			_db.Add(new UserPassword {
				UserId = _me.Id,
				Password = Crypto.SHA1(newPassword)
			});

			await _db.SaveChangesAsync();
			return new Success();
		}
	}
}
