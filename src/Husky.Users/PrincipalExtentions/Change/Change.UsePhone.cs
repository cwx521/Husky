using System.Linq;
using System.Threading.Tasks;
using Husky.Users.Data;
using Husky.TwoFactor;

namespace Husky.Principal
{
	partial class ChangeManager
	{
		public async Task<Result> UsePhone(string newNumber, string verificationCode) {
			if ( _me.IsAnonymous ) {
				return new Failure("需要先登录");
			}
			if ( _db.UserPhones.Any(x => x.UserId != _me.Id && x.Number == newNumber) ) {
				return new Failure($"{newNumber.Mask()} 已被其它帐号使用");
			}

			var validationModel = new TwoFactorModel { SendTo = newNumber, Code = verificationCode };
			var validationResult = await _me.TwoFactor().VerifyTwoFactorCode(validationModel, true);

			if ( !validationResult.Ok ) {
				return validationResult;
			}

			var userPhone = _db.UserPhones.Find(_me.Id);
			if ( userPhone == null ) {
				userPhone = new UserPhone {
					UserId = _me.Id
				};
				_db.Add(userPhone);
			}

			userPhone.Number = newNumber;
			userPhone.IsVerified = true;

			await _db.SaveChangesAsync();
			return new Success();
		}
	}
}
