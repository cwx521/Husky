using System;
using System.Linq;
using System.Threading.Tasks;
using Husky;
using Husky.Principal.Users.Data;
using Husky.WeChatIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal.Users
{
	public partial class UserAuthFunctions
	{
		public async Task<Result> SignInWithWeChatAsync(string wechatCode, WeChatAppIdSecret idSecret) {
			if ( idSecret.Type == null ) {
				throw new ArgumentException($"Unknown {nameof(WeChatAppIdSecret)}.{nameof(WeChatAppIdSecret.Type)}");
			}

			var wechat = _me.ServiceProvider.GetRequiredService<WeChatService>().User();

			var accessToken = await wechat.GetUserAccessTokenAsync(wechatCode, idSecret);
			if ( !accessToken.Ok || accessToken.Data == null ) {
				return new Failure(LoginResult.FailureWeChatRequestToken.ToLabel());
			}

			var user = _db.Users
				.AsNoTracking()
				.Where(x => x.WeChat != null && x.WeChat.OpenIds.Any(y => y.OpenIdValue == accessToken.Data.OpenId))
				.SingleOrDefault();

			//用户还不存在，即该微信账号第一次登录，进一步读取用户资料完成自动注册
			if ( user == null ) {

				var wechatUser = await wechat.GetUserInfoAsync(accessToken.Data);
				if ( !wechatUser.Ok || wechatUser.Data == null ) {
					return new Failure(LoginResult.FailureWeChatRequestUserInfo.ToLabel());
				}

				//寻找用户，看该微信账号是否有相同UnionId
				if ( !string.IsNullOrEmpty(wechatUser.Data.UnionId) ) {
					user = _db.Users
						.Include(x => x.WeChat)
						.ThenInclude(x => x!.OpenIds)
						.Where(x => x.WeChat != null)
						.Where(x => x.WeChat!.UnionId == wechatUser.Data.UnionId)
						.SingleOrDefault();
				}

				//如果仍然没找到已注册用户，判断用户当前是否已经通过其它方式登录，是的话直接使用该用户身份
				if ( user == null && _me.IsAuthenticated ) {
					user = _db.Users
						.Include(x => x.WeChat)
						.ThenInclude(x => x!.OpenIds)
						.Where(x => x.Id == _me.Id)
						.SingleOrDefault();
				}

				//如果都没有，就新建用户
				if ( user == null ) {
					user = new User();
					_db.Users.Add(user);
				}
				else {
					//用户记录是异常状态时，阻止获得登录身份
					if ( user.Status == RowStatus.Suspended ) {
						return await AddLoginRecordAsync(LoginResult.RejectedAccountSuspended, "WeChatApi", user.Id);
					}
					if ( user.Status == RowStatus.Deleted ) {
						return await AddLoginRecordAsync(LoginResult.RejectedAccountDeleted, "WeChatApi", user.Id);
					}
					if ( user.Status != RowStatus.Active ) {
						return await AddLoginRecordAsync(LoginResult.RejectedAccountInactive, "WeChatApi", user.Id);
					}
				}

				//更新 User 表字段
				user.DisplayName ??= wechatUser.Data.NickName.Left(36);
				user.PhotoUrl ??= wechatUser.Data.HeadImageUrl;

				//更新 UserWeChat 表字段
				user.WeChat ??= new UserWeChat();

				if ( !user.WeChat.OpenIds.Any(x => x.OpenIdValue == wechatUser.Data.OpenId) ) {
					user.WeChat.OpenIds.Add(new UserWeChatOpenId {
						OpenIdType = (WeChatField)(int)idSecret.Type,
						OpenIdValue = wechatUser.Data.OpenId
					});
				}
				user.WeChat.UnionId = wechatUser.Data.UnionId;
				user.WeChat.NickName = wechatUser.Data.NickName.Left(36)!;
				user.WeChat.Sex = wechatUser.Data.Sex;
				user.WeChat.HeadImageUrl = wechatUser.Data.HeadImageUrl;
				user.WeChat.Province = wechatUser.Data.Province?.Left(24);
				user.WeChat.City = wechatUser.Data.City?.Left(24);
				user.WeChat.Country = wechatUser.Data.Country?.Left(24);

				await _db.Normalize().SaveChangesAsync();
			}

			_me.Id = user.Id;
			_me.DisplayName = user.DisplayName ?? $"User#{user.Id}";
			_me.IdentityManager?.SaveIdentity(_me);

			return await AddLoginRecordAsync(LoginResult.Success, "WeChatApi", user.Id);
		}
	}
}
