using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Husky.Mail;
using Husky.Principal;
using Husky.Sms;
using Husky.TwoFactor.Data;
using Microsoft.EntityFrameworkCore;

namespace Husky.TwoFactor
{
	public sealed partial class TwoFactorManager : ITwoFactorManager
	{
		public TwoFactorManager(ITwoFactorDbContext twoFactorDb, IPrincipalUser principal, ISmsSender? smsSender, IMailSender? mailSender) {
			if ( smsSender == null && mailSender == null ) {
				throw new ArgumentNullException(null, $"At least to configure one of {smsSender} or {mailSender}");
			}
			_twoFactorDb = twoFactorDb;
			_me = principal;
			_smsSender = smsSender;
			_mailSender = mailSender;
		}

		private readonly ITwoFactorDbContext _twoFactorDb;
		private readonly IPrincipalUser _me;
		private readonly ISmsSender? _smsSender;
		private readonly IMailSender? _mailSender;

		public async Task<Result> SendCodeAsync(string mobileNumberOrEmailAddress, string? overrideMessageTemplateWithCodeArg0 = null, string? overrideSmsTemplateAlias = null, string? overrideSmsSignName = null) {
			if ( mobileNumberOrEmailAddress == null ) {
				throw new ArgumentNullException(nameof(mobileNumberOrEmailAddress));
			}

			var isEmail = mobileNumberOrEmailAddress.IsEmail();
			var isMobile = mobileNumberOrEmailAddress.IsMainlandMobile();

			if ( !isEmail && !isMobile ) {
				return new Failure($"无法送达 '{mobileNumberOrEmailAddress}'");
			}

			if ( isMobile ) {
				var sentWithinMinute = await _twoFactorDb.TwoFactorCodes
					.AsNoTracking()
					.Where(x => x.SentTo == mobileNumberOrEmailAddress)
					.Where(x => x.CreatedTime > DateTime.Now.AddMinutes(-1))
					.AnyAsync();

				if ( sentWithinMinute ) {
					return new Failure("请求过于频繁");
				}
			}

			var code = new TwoFactorCode {
				UserId = _me.Id,
				AnonymousId = _me.AnonymousId,
				Code = new Random().Next(0, 1000000).ToString("D6"),
				SentTo = mobileNumberOrEmailAddress
			};
			_twoFactorDb.TwoFactorCodes.Add(code);
			await _twoFactorDb.Normalize().SaveChangesAsync();

			if ( isEmail ) {
				if ( _mailSender == null ) {
					throw new Exception($"Required to inject service {typeof(IMailSender).Assembly.GetName()}");
				}
				var wrappedSignName = overrideSmsSignName == null ? null : $"【{overrideSmsSignName}】 ";
				var content = string.Format(overrideMessageTemplateWithCodeArg0 ?? "{wrappedSignName}验证码： {0}", code.Code);
				await _mailSender.SendAsync($"{wrappedSignName}动态验证码", content, mobileNumberOrEmailAddress);
			}

			else if ( isMobile ) {
				if ( _smsSender == null ) {
					throw new Exception($"Required to inject service {typeof(ISmsSender).Assembly.GetName()}");
				}
				var shortMessage = new SmsBody {
					SignName = overrideSmsSignName,
					Template = overrideMessageTemplateWithCodeArg0,
					TemplateAlias = overrideSmsTemplateAlias,
					Parameters = new Dictionary<string, string> {
						{ "code", code.Code }
					}
				};
				await _smsSender.SendAsync(shortMessage, mobileNumberOrEmailAddress);
			}

			return new Success();
		}

		public async Task<Result> SendCodeThroughSmsAsync(string mobileNumber, string? overrideMessageTemplateWithCodeArg0 = null, string? overrideSmsTemplateAlias = null, string? overrideSmsSignName = null) {
			if ( !mobileNumber.IsMainlandMobile() ) {
				return new Failure($"无法送达 '{mobileNumber}'");
			}
			return await SendCodeAsync(mobileNumber, overrideMessageTemplateWithCodeArg0, overrideSmsTemplateAlias, overrideSmsSignName);
		}

		public async Task<Result> SendCodeThroughEmailAsync(string emailAddress, string? messageTemplateWithCodeArg0 = null) {
			if ( !emailAddress.IsEmail() ) {
				return new Failure($"无法送达 '{emailAddress}'");
			}
			return await SendCodeAsync(emailAddress, messageTemplateWithCodeArg0, null, null);
		}

		public async Task<Result> VerifyCodeAsync(string sentTo, string code, bool setIntoUsedAfterVerifying, int withinMinutes = 15) {
			if ( sentTo == null ) {
				throw new ArgumentNullException(nameof(sentTo));
			}
			if ( code == null ) {
				throw new ArgumentNullException(nameof(code));
			}

			var record = await _twoFactorDb.TwoFactorCodes
				.Where(x => x.IsUsed == false)
				.Where(x => x.CreatedTime > DateTime.Now.AddMinutes(0 - withinMinutes))
				.Where(x => x.SentTo == sentTo)
				.Where(x => x.UserId == _me.Id || x.AnonymousId == _me.AnonymousId)
				.OrderByDescending(x => x.Id)
				.FirstOrDefaultAsync();

			if ( record == null ) {
				return new Failure("验证码匹配失败");
			}
			if ( record.ErrorTimes > 10 || string.Compare(code, record.Code, true) != 0 ) {
				record.ErrorTimes++;
				await _twoFactorDb.Normalize().SaveChangesAsync();
				return new Failure("验证码输入错误");
			}
			if ( setIntoUsedAfterVerifying ) {
				record.IsUsed = true;
				await _twoFactorDb.Normalize().SaveChangesAsync();
			}
			return new Success();
		}

		public async Task<Result> VerifyCodeAsync(ITwoFactorModel model, bool setIntoUsedAfterVerifying, int withinMinutes = 15) {
			if ( model == null ) {
				throw new ArgumentNullException(nameof(model));
			}
			return await VerifyCodeAsync(model.SendTo, model.Code, setIntoUsedAfterVerifying, withinMinutes);
		}
	}
}