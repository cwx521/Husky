using System.Threading.Tasks;

namespace Husky.TwoFactor
{
	public interface ITwoFactorManager
	{
		Task<Result> SendCodeAsync(string mobileNumberOrEmailAddress,
			string? overrideMessageTemplateWithCodeArg0 = null,
			string? overrideSmsTemplateAlias = null,
			string? overrideSmsSignName = null);

		Task<Result> SendCodeThroughSmsAsync(string mobileNumber,
			string? overrideMessageTemplateWithCodeArg0 = null,
			string? overrideSmsTemplateAlias = null,
			string? overrideSmsSignName = null);

		Task<Result> SendCodeThroughEmailAsync(string emailAddress,
			string? messageTemplateWithCodeArg0 = null,
			string? overrideSmsSignName = null);

		Task<Result> VerifyCodeAsync(string sentTo, string code, bool setIntoUsedIfSucceed, int withinMinutes = 15);

		Task<Result> VerifyCodeAsync(ITwoFactorModel model, bool setIntoUsedIfSucceed, int withinMinutes = 15);
	}
}