using System.Threading.Tasks;
using Husky.Sms;

namespace Husky.TwoFactor
{
	public interface ITwoFactorManager
	{
		Task<Result> SendCode(string mobileNumberOrEmailAddress,
			string? overrideMessageTemplateWithCodeArg0 = null,
			string? overrideSmsTemplateAlias = null,
			string? overrideSmsSignName = null);

		Task<Result> SendCodeThroughSms(string mobileNumber,
			string? overrideMessageTemplateWithCodeArg0 = null,
			string? overrideSmsTemplateAlias = null,
			string? overrideSmsSignName = null);

		Task<Result> SendCodeThroughEmail(string emailAddress, string? messageTemplateWithCodeArg0 = null);

		Task<Result> VerifyCode(string sentTo, string code, bool setIntoUsedAfterVerifying, int withinMinutes = 15);

		Task<Result> VerifyCode(ITwoFactorModel model, bool setIntoUsedAfterVerifying, int withinMinutes = 15);
	}
}