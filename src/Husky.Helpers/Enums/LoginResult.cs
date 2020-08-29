using Husky;

namespace Shipping.Data
{
	public enum LoginResult
	{
		[Label("成功")]
		Success,

		[Label("帐号格式不正确")]
		InvalidInput,

		[Label("帐号不存在")]
		AccountNotFound,

		[Label("密码不正确")]
		ErrorPassword,

		[Label("图形验证码不正确")]
		ErrorCaptcha,

		[Label("连续失败次数过多")]
		RejectedContinuousAttemption,

		[Label("密码已过期")]
		RejectedPasswordExpiration,

		[Label("帐号未激活")]
		RejectedAccountInactive,

		[Label("登录被拒绝")]
		RejectedOtherReason,

		[Label("帐号已被停用")]
		RejectedAccountSuspended
	}
}