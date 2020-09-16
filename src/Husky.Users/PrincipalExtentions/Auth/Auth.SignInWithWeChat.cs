using System;
using System.Linq;
using System.Threading.Tasks;
using Husky;
using Husky.Users.Data;
using Husky.WeChatIntegration;
using Microsoft.EntityFrameworkCore;

namespace Husky.Principal
{
	partial class AuthManager
	{
		public async Task<LoginResult> SignInWithWeChat(string wechatCode, WeChatAppIdSecret idSecret) {
			if ( _wechat == null ) {
				throw new Exception($"未添加微信服务组件 {typeof(WeChatService).Assembly.GetName()}");
			}
			if ( idSecret.Type == null ) {
				throw new ArgumentException($"未指明 {nameof(WeChatAppIdSecret)}.{nameof(WeChatAppIdSecret.Type)}");
			}

			var wechatUserService = _wechat.UserService();

			var accessToken = wechatUserService.GetUserAccessToken(wechatCode, idSecret);
			if ( accessToken == null ) {
				return LoginResult.FailureWeChatRequestToken;
			}
			var wechatUser = wechatUserService.GetUserInfo(accessToken);
			if ( wechatUser == null ) {
				return LoginResult.FailureWeChatRequestUserInfo;
			}

			//寻找用户，看该微信账号是否已经注册过
			var user = _db.Users
				.Include(x => x.WeChat)
				.Where(x => x.WeChat != null)
				.Where(x => x.WeChat!.UnionId == wechatUser.UnionId || x.WeChat.OpenIds.Any(y => y.OpenIdValue == wechatUser.OpenId))
				.SingleOrDefault();

			//如果通过该微信账号没找到已注册用户，判断用户当前是否已经通过其它方式登录，是的话直接使用该用户身份
			if ( user == null && _me.IsAuthenticated ) {
				user = _db.Users
					.Where(x => x.WeChat == null)
					.Where(x => x.Id == _me.Id)
					.SingleOrDefault();
			}

			//如果都没有，就新建用户
			if ( user == null ) {
				user = new User();
				_db.Add(user);
			}
			user.WeChat ??= new UserWeChat();

			//更新 User 表字段
			user.DisplayName ??= wechatUser.NickName.Left(36);
			user.PhotoUrl ??= wechatUser.HeadImageUrl;

			//更新 UserWeChat 表字段		
			user.WeChat.OpenIds.Add(new UserWeChatOpenId {
				OpenIdType = (WeChatOpenIdType)(int)idSecret.Type,
				OpenIdValue = wechatUser.OpenId
			});
			user.WeChat.UnionId = wechatUser.UnionId;
			user.WeChat.NickName = wechatUser.NickName.Left(36)!;
			user.WeChat.Sex = wechatUser.Sex;
			user.WeChat.HeadImageUrl = wechatUser.HeadImageUrl;
			user.WeChat.Province = wechatUser.Province?.Left(24);
			user.WeChat.City = wechatUser.City?.Left(24);
			user.WeChat.Country = wechatUser.Country?.Left(24);

			//用户记录是异常状态时，阻止获得登录身份
			if ( user.Status == RowStatus.Suspended ) {
				return await AddLoginRecord(LoginResult.RejectedAccountSuspended, "WeChatApi", user.Id);
			}
			if ( user.Status == RowStatus.DeletedByAdmin || user.Status == RowStatus.DeletedByUser ) {
				return await AddLoginRecord(LoginResult.RejectedAccountDeleted, "WeChatApi", user.Id);
			}
			if ( user.Status != RowStatus.Active ) {
				return await AddLoginRecord(LoginResult.RejectedAccountInactive, "WeChatApi", user.Id);
			}

			await _db.SaveChangesAsync();

			_me.Id = user.Id;
			_me.DisplayName = user.DisplayName!;
			_me.IdentityManager.SaveIdentity(_me);

			return await AddLoginRecord(LoginResult.Success, "WeChatApi", user.Id);
		}
	}
}
