using System.Linq;
using System.Threading.Tasks;
using Husky.Principal.Users.Data;
using Husky.TwoFactor;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal.Users
{
	public partial class UserProfileFunctions
	{
		public async Task<Result> SavePhoneAsync(string newNumber, string? verificationCode) {
			if ( _me.IsAnonymous ) {
				return new Failure("需要先登录");
			}

			if ( newNumber.StartsWith('+') ) {
				newNumber = newNumber.Substring(1);
			}
			if ( newNumber.StartsWith("86") ) {
				newNumber = newNumber.Substring(2);
			}

			if ( !newNumber.IsMainlandMobile() ) {
				return new Failure("格式错误");
			}
			if ( _db.UserPhones.Any(x => x.UserId != _me.Id && x.Number == newNumber) ) {
				return new Failure($"{newNumber.Mask()} 已被其它帐号使用");
			}

			if ( verificationCode != null ) {
				var twoFactor = _me.ServiceProvider.GetRequiredService<ITwoFactorManager>();
				var verifyResult = await twoFactor.VerifyCodeAsync(newNumber, verificationCode, true);
				if ( !verifyResult.Ok ) {
					return verifyResult;
				}
			}

			var userPhone = await _db.UserPhones.FindAsync(_me.Id);
			if ( userPhone == null ) {
				userPhone = new UserPhone {
					UserId = _me.Id
				};
				_db.UserPhones.Add(userPhone);
			}
			else if ( userPhone.Number != newNumber ) {
				userPhone.Number = newNumber;
				userPhone.IsVerified = verificationCode != null;
			}

			await _db.Normalize().SaveChangesAsync();
			return new Success();
		}
	}
}
