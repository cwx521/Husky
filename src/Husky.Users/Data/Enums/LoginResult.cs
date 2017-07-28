namespace Husky.Users.Data
{
	public enum LoginResult
	{
		Success,
		InvalidInput,
		AccountNotFound,
		ErrorPassword,
		ErrorCaptcha,
		RejectedContinuousAttemption,
		RejectedPasswordExpiration,
		RejectedAccountInactive,
		RejectedOtherReason
	}
}