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
		public async Task<Result> SignInWithWeChat(string wechatCode, WeChatAppIdSecret idSecret) {
			if ( idSecret.Type == null ) {
				throw new ArgumentException($"未指明 {nameof(WeChatAppIdSecret)}.{nameof(WeChatAppIdSecret.Type)}");
			}

			var wechat = _me.ServiceProvider.GetService<WeChatService>();
			if ( wechat == null ) {
				throw new Exception($"缺少微信服务组件 {typeof(WeChatService).Assembly.GetName()}");
			}

			var wechatUserService = wechat.User();

			var accessToken = wechatUserService.GetUserAccessToken(wechatCode, idSecret);
			if ( accessToken == null ) {
				return new Failure(LoginResult.FailureWeChatRequestToken.ToLabel());
			}
			var wechatUser = wechatUserService.GetUserInfo(accessToken);
			if ( wechatUser == null ) {
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
					return await AddLoginRecord(LoginResult.RejectedAccountSuspended, "WeChatApi", user.Id);
				}
				if ( user.Status == RowStatus.Deleted ) {
					return await AddLoginRecord(LoginResult.RejectedAccountDeleted, "WeChatApi", user.Id);
				}
				if ( user.Status != RowStatus.Active ) {
					return await AddLoginRecord(LoginResult.RejectedAccountInactive, "WeChatApi", user.Id);
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

			return await AddLoginRecord(LoginResult.Success, "WeChatApi", user.Id);
		}
	}
}
