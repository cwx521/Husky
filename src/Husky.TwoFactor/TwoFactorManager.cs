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
			if (smsSender == null && mailSender == null) {
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

		public async Task<Result> SendCodeAsync(string cellphoneOrEmail, string? overrideContentTemplateWithArg0 = null, string? overrideTemplateCode = null, string? overrideSignName = null) {
			if (cellphoneOrEmail == null) {
				throw new ArgumentNullException(nameof(cellphoneOrEmail));
			}

			var isEmail = cellphoneOrEmail.IsEmail();
			var isCellphone = cellphoneOrEmail.IsChinaMainlandCellphone();

			if (!isEmail && !isCellphone) {
				return new Failure($"无法送达 '{cellphoneOrEmail}'");
			}

			var justSentWithinMinute = await _twoFactorDb.TwoFactorCodes
				.AsNoTracking()
				.Where(x => x.SentTo == cellphoneOrEmail)
				.Where(x => x.CreatedTime > DateTime.Now.AddMinutes(-1))
				.AnyAsync();

			if (justSentWithinMinute) {
				return new Failure("过于频繁，请1分钟后再试");
			}

			var code = new TwoFactorCode {
				UserId = _me.Id,
				AnonymousId = _me.AnonymousId,
				Code = new Random().Next(0, 1000000).ToString("D6"),
				SentTo = cellphoneOrEmail
			};
			_twoFactorDb.TwoFactorCodes.Add(code);
			await _twoFactorDb.Normalize().SaveChangesAsync();

			if (isEmail) {
				if (_mailSender == null) {
					throw new Exception($"Required to inject service {nameof(IMailSender)}");
				}
				var signName = overrideSignName == null ? null : $"【{overrideSignName}】 ";
				var template = overrideContentTemplateWithArg0 ?? "验证码： {0}";
				var content = string.Format(template, code.Code);
				await _mailSender.SendAsync($"{signName}动态验证码", content, cellphoneOrEmail);
			}
			else if (isCellphone) {
				if (_smsSender == null) {
					throw new Exception($"Required to inject service {nameof(IMailSender)}");
				}
				var sms = new SmsBody {
					SignName = overrideSignName,
					Template = overrideContentTemplateWithArg0,
					TemplateCode = overrideTemplateCode,
					Parameters = new Dictionary<string, string> {
						["code"] = code.Code
					}
				};
				await _smsSender.SendAsync(sms, cellphoneOrEmail);
			}
			return new Success();
		}

		public async Task<Result> SendCodeThroughSmsAsync(string cellphone, string? overrideContentTemplateWithArg0 = null, string? overrideTemplateCode = null, string? overrideSignName = null) {
			if (!cellphone.IsChinaMainlandCellphone()) {
				return new Failure($"无法送达 '{cellphone}'");
			}
			return await SendCodeAsync(cellphone, overrideContentTemplateWithArg0, overrideTemplateCode, overrideSignName);
		}

		public async Task<Result> SendCodeThroughEmailAsync(string emailAddress, string? overrideContentTemplateWithArg0 = null, string? overrideSignName = null) {
			if (!emailAddress.IsEmail()) {
				return new Failure($"无法送达 '{emailAddress}'");
			}
			return await SendCodeAsync(emailAddress, overrideContentTemplateWithArg0, null, overrideSignName);
		}

		public async Task<Result> VerifyCodeAsync(string sentTo, string code, bool setIntoUsedIfSuccess, int codeExpirationMinutes = 15) {
			if (sentTo == null) {
				throw new ArgumentNullException(nameof(sentTo));
			}
			if (code == null) {
				throw new ArgumentNullException(nameof(code));
			}

			var record = await _twoFactorDb.TwoFactorCodes
				.Where(x => x.IsUsed == false)
				.Where(x => x.SentTo == sentTo)
				.Where(x => x.UserId == _me.Id || x.AnonymousId == _me.AnonymousId)
				.Where(x => x.CreatedTime > DateTime.Now.AddMinutes(0 - codeExpirationMinutes))
				.OrderByDescending(x => x.Id)
				.FirstOrDefaultAsync();

			if (record == null) {
				return new Failure("验证码不正确");
			}
			if (record.ErrorTimes > 10 || string.Compare(code, record.Code, true) != 0) {
				record.ErrorTimes++;
				await _twoFactorDb.Normalize().SaveChangesAsync();
				return new Failure("验证码错误");
			}
			if (setIntoUsedIfSuccess) {
				record.IsUsed = true;
				await _twoFactorDb.Normalize().SaveChangesAsync();
				_twoFactorDb.Normalize().ChangeTracker.Clear();
			}
			return new Success();
		}
	}
}