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
		public TwoFactorManager(ITwoFactorDbContext twoFactorDb, IPrincipalUser principal, ISmsSender smsSender)
			: this(twoFactorDb, principal, smsSender, null) {
		}

		public TwoFactorManager(ITwoFactorDbContext twoFactorDb, IPrincipalUser principal, IMailSender mailSender)
			: this(twoFactorDb, principal, null, mailSender) {
		}

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

		private async Task<Result> SendCodeAsync(string mobileNumberOrEmailAddress, string? overrideMessageTemplateWithCodeArg0 = null, string? overrideSmsTemplateAlias = null, string? overrideSmsSignName = null) {
			if ( mobileNumberOrEmailAddress == null ) {
				throw new ArgumentNullException(nameof(mobileNumberOrEmailAddress));
			}

			var isEmail = mobileNumberOrEmailAddress.IsEmail();
			var isMobile = mobileNumberOrEmailAddress.IsMobileNumber();

			if ( !isEmail && !isMobile ) {
				return new Failure($"无法送达 '{mobileNumberOrEmailAddress}'");
			}

			var justSentWithinMinute = await _twoFactorDb.TwoFactorCodes
				.AsNoTracking()
				.Where(x => x.SentTo == mobileNumberOrEmailAddress)
				.Where(x => x.CreatedTime > DateTime.Now.AddMinutes(-1))
				.AnyAsync();

			if ( justSentWithinMinute ) {
				return new Failure("过于频繁，请1分钟后再试");
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
					throw new Exception($"Required to inject service {nameof(IMailSender)}");
				}
				var wrappedSignName = overrideSmsSignName == null ? null : $"【{overrideSmsSignName}】 ";
				var content = string.Format(overrideMessageTemplateWithCodeArg0 ?? "{wrappedSignName}验证码： {0}", code.Code);
				await _mailSender.SendAsync($"{wrappedSignName}动态验证码", content, mobileNumberOrEmailAddress);
			}
			else if ( isMobile ) {
				if ( _smsSender == null ) {
					throw new Exception($"Required to inject service {nameof(IMailSender)}");
				}
				var shortMessage = new SmsBody {
					SignName = overrideSmsSignName,
					Template = overrideMessageTemplateWithCodeArg0,
					TemplateAlias = overrideSmsTemplateAlias,
					Parameters = new Dictionary<string, string> {
						["code"] = code.Code
					}
				};
				await _smsSender.SendAsync(shortMessage, mobileNumberOrEmailAddress);
			}
			return new Success();
		}

		public async Task<Result> SendCodeThroughSmsAsync(string mobileNumber, string? overrideMessageTemplateWithCodeArg0 = null, string? overrideSmsTemplateAlias = null, string? overrideSmsSignName = null) {
			if ( !mobileNumber.IsMobileNumber() ) {
				return new Failure($"无法送达 '{mobileNumber}'");
			}
			return await SendCodeAsync(mobileNumber, overrideMessageTemplateWithCodeArg0, overrideSmsTemplateAlias, overrideSmsSignName);
		}

		public async Task<Result> SendCodeThroughEmailAsync(string emailAddress, string? messageTemplateWithCodeArg0 = null, string? overrideSmsSignName = null) {
			if ( !emailAddress.IsEmail() ) {
				return new Failure($"无法送达 '{emailAddress}'");
			}
			return await SendCodeAsync(emailAddress, messageTemplateWithCodeArg0, null, overrideSmsSignName);
		}

		public async Task<Result> VerifyCodeAsync(string sentTo, string code, bool setIntoUsedIfSuccess, int codeUseableWithinMinutes = 15) {
			if ( sentTo == null ) {
				throw new ArgumentNullException(nameof(sentTo));
			}
			if ( code == null ) {
				throw new ArgumentNullException(nameof(code));
			}

			var record = await _twoFactorDb.TwoFactorCodes
				.Where(x => x.IsUsed == false)
				.Where(x => x.SentTo == sentTo)
				.Where(x => x.UserId == _me.Id || x.AnonymousId == _me.AnonymousId)
				.Where(x => x.CreatedTime > DateTime.Now.AddMinutes(0 - codeUseableWithinMinutes))
				.OrderByDescending(x => x.Id)
				.FirstOrDefaultAsync();

			if ( record == null ) {
				return new Failure("验证码不正确");
			}
			if ( record.ErrorTimes > 10 || string.Compare(code, record.Code, true) != 0 ) {
				record.ErrorTimes++;
				await _twoFactorDb.Normalize().SaveChangesAsync();
				return new Failure("验证码错误");
			}
			if ( setIntoUsedIfSuccess ) {
				record.IsUsed = true;
				await _twoFactorDb.Normalize().SaveChangesAsync();
				_twoFactorDb.Normalize().ChangeTracker.Clear();
			}
			return new Success();
		}
	}
}