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
	public partial class UserAuthManager
	{
		public async Task<Result> SignInWithWeChatAsync(string wechatCode, WeChatAppIdSecret idSecret) {
			if ( idSecret.Type == null ) {
				throw new ArgumentException($"Unknown {nameof(WeChatAppIdSecret)}.{nameof(WeChatAppIdSecret.Type)}");
			}

			var wechat = _me.ServiceProvider.GetRequiredService<WeChatService>().User();

			var accessToken = await wechat.GetUserAccessTokenAsync(wechatCode, idSecret);
			if ( !accessToken.Ok ) {
				return new Failure(LoginResult.FailureWeChatRequestToken.ToLabel());
			}

			var wechatUser = await wechat.GetUserInfoAsync(accessToken);
			if ( !wechatUser.Ok ) {
				return new Failure(LoginResult.FailureWeChatRequestUserInfo.ToLabel());
			}

			//寻找用户，看该微信账号是否已经注册过
			var user = _db.Users
				.Include(x => x.WeChat)
				.Where(x => x.WeChat != null)
				.Where(x => x.WeChat!.UnionId == wechatUser.UnionId || x.WeChat.OpenIds.Any(y => y.OpenIdValue == wechatUser.OpenId))
				.SingleOrDefault();

			//如果通过传入微信信息没找到已注册用户，判断用户当前是否已经通过其它方式登录，是的话直接使用该用户身份
			if ( user == null && _me.IsAuthenticated ) {
				user = _db.Users
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
			user.DisplayName ??= wechatUser.NickName.Left(36);
			user.PhotoUrl ??= wechatUser.HeadImageUrl;

			//更新 UserWeChat 表字段
			user.WeChat ??= new UserWeChat();

			if ( !user.WeChat.OpenIds.Any(x => x.OpenIdValue == wechatUser.OpenId) ) {
				user.WeChat.OpenIds.Add(new UserWeChatOpenId {
					OpenIdType = (WeChatField)(int)idSecret.Type,
					OpenIdValue = wechatUser.OpenId
				});
			}
			user.WeChat.UnionId = wechatUser.UnionId;
			user.WeChat.NickName = wechatUser.NickName.Left(36)!;
			user.WeChat.Sex = wechatUser.Sex;
			user.WeChat.HeadImageUrl = wechatUser.HeadImageUrl;
			user.WeChat.Province = wechatUser.Province?.Left(24);
			user.WeChat.City = wechatUser.City?.Left(24);
			user.WeChat.Country = wechatUser.Country?.Left(24);

			await _db.Normalize().SaveChangesAsync();

			_me.Id = user.Id;
			_me.DisplayName = user.DisplayName!;
			_me.IdentityManager?.SaveIdentity(_me);

			return await AddLoginRecordAsync(LoginResult.Success, "WeChatApi", user.Id);
		}
	}
}
