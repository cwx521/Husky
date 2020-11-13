using System.Linq;
using System.Threading.Tasks;
using Husky.Principal.Users.Data;
using Husky.TwoFactor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal.Users
{
	public partial class UserAuthFunctions
	{
		public async Task<Result> SignInWithPhoneAsync(string mobile, string verificationCode) {
			if ( string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(verificationCode) ) {
				return new Failure(LoginResult.InvalidInput.ToLabel());
			}
			if ( !mobile.IsMainlandMobile() ) {
				return new Failure(LoginResult.InvalidInput.ToLabel());
			}

			//读取用户记录
			var twoFactor = _me.ServiceProvider.GetRequiredService<ITwoFactorManager>();
			var verifyResult = await twoFactor.VerifyCodeAsync(mobile, verificationCode, true);
			var user = await _db.Users.SingleOrDefaultAsync(x => x.Phone != null && x.Phone.Number == mobile);

			//验证码不通过
			if ( !verifyResult.Ok ) {
				return await AddLoginRecordAsync(LoginResult.ErrorTwoFactorCode, mobile, user?.Id);
			}

			//如果通过手机号没找到已注册用户，判断用户当前是否已经通过其它方式登录，是的话直接使用该用户身份
			if ( user == null && _me.IsAuthenticated ) {
				user = _db.Users
					.Where(x => x.Phone == null)
					.Where(x => x.Id == _me.Id)
					.SingleOrDefault();
			}

			//如果仍然没找到，自动注册新建用户
			if ( user == null ) {
				user = new User {
					Phone = new UserPhone {
						Number = mobile,
						IsVerified = true
					}
				};
				_db.Users.Add(user);
			}
			else {
				//用户记录是异常状态时，阻止获得登录身份
				if ( user.Status == RowStatus.Suspended ) {
					return await AddLoginRecordAsync(LoginResult.RejectedAccountSuspended, mobile, user.Id);
				}
				if ( user.Status == RowStatus.Deleted ) {
					return await AddLoginRecordAsync(LoginResult.RejectedAccountDeleted, mobile, user.Id);
				}
				if ( user.Status != RowStatus.Active ) {
					return await AddLoginRecordAsync(LoginResult.RejectedAccountInactive, mobile, user.Id);
				}
			}

			await _db.Normalize().SaveChangesAsync();

			_me.Id = user.Id;
			_me.DisplayName = user.DisplayName ?? mobile.Mask()!;
			_me.IsConsolidated = true;
			_me.IdentityManager?.SaveIdentity(_me);

			return await AddLoginRecordAsync(LoginResult.Success, mobile, user.Id);
		}
	}
}
