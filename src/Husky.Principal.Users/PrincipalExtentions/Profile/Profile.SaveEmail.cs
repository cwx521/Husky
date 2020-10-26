using System.Linq;
using System.Threading.Tasks;
using Husky.Principal.Users.Data;
using Husky.TwoFactor;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal.Users
{
	public partial class UserProfileManager
	{
		public async Task<Result> SaveEmailAsync(string newEmailAddress, string? verificationCode = null) {
			if ( _me.IsAnonymous ) {
				return new Failure("需要先登录");
			}
			if ( !newEmailAddress.IsEmail() ) {
				return new Failure("格式错误");
			}
			if ( _db.UserEmails.Any(x => x.UserId != _me.Id && x.EmailAddress == newEmailAddress) ) {
				return new Failure($"{newEmailAddress.Mask()} 已被其它帐号使用");
			}

			if ( verificationCode != null ) {
				var twoFactor = _me.ServiceProvider.GetRequiredService<ITwoFactorManager>();
				var verifyResult = await twoFactor.VerifyCodeAsync(newEmailAddress, verificationCode, true);
				if ( !verifyResult.Ok ) {
					return verifyResult;
				}
			}

			var userEmail = await _db.UserEmails.FindAsync(_me.Id);
			if ( userEmail == null ) {
				userEmail = new UserEmail {
					UserId = _me.Id,
				};
				_db.UserEmails.Add(userEmail);
			}
			else if ( userEmail.EmailAddress != newEmailAddress ) {
				userEmail.EmailAddress = newEmailAddress;
				userEmail.IsVerified = verificationCode != null;
			}

			await _db.Normalize().SaveChangesAsync();
			return new Success();
		}
	}
}
