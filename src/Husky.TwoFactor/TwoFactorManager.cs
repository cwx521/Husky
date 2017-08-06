using System;
using System.Linq;
using System.Threading.Tasks;
using Husky.Authentication.Abstractions;
using Husky.Mail.Abstractions;
using Husky.Sms.Abstractions;
using Husky.Sugar;
using Husky.TwoFactor.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.TwoFactor
{
	public sealed partial class TwoFactorManager
	{
		public TwoFactorManager(IServiceProvider serviceProvider) {
			_db = serviceProvider.GetRequiredService<TwoFactorDbContext>();
			_my = serviceProvider.GetRequiredService<IPrincipal>();
			_site = serviceProvider.GetRequiredService<IConfiguration>().GetSection(SiteVariables.ConfigurationSectionName).Get<SiteVariables>();

			_mailSender = serviceProvider.GetService<IMailSender>();
			_smsSender = serviceProvider.GetService<ISmsSender>();
		}

		readonly TwoFactorDbContext _db;
		readonly IPrincipal _my;
		readonly SiteVariables _site;

		readonly IMailSender _mailSender;
		readonly ISmsSender _smsSender;

		public async Task<Result> RequestTwoFactorCode(string emailOrMobile, TwoFactorPurpose purpose, string messageTemplateArg0IsTheCode = null) {
			if ( emailOrMobile == null ) {
				throw new ArgumentNullException(nameof(emailOrMobile));
			}

			var isEmail = emailOrMobile.IsEmail();
			var isMobile = emailOrMobile.IsMainlandMobile();

			if ( !isEmail && !isMobile ) {
				return new Failure("接收对象 '{0}' 格式无效。".Xslate(emailOrMobile));
			}

			var code = new TwoFactorCode {
				UserId = _my.Id<Guid>(),
				Code = new Random().Next(0, 1000000).ToString().PadLeft(6, '0'),
				Purpose = purpose,
				SentTo = emailOrMobile
			};
			_db.Add(code);
			await _db.SaveChangesAsync();

			var template = messageTemplateArg0IsTheCode ?? string.Format("【{0}】动态验证码：{{0}}".Xslate(), _site.SiteName);
			var content = string.Format(template, code.Code);

			if ( isEmail ) {
				await SendViaEmail(content, emailOrMobile);
			}
			else if ( isMobile ) {
				await SendViaSms(content, emailOrMobile);
			}
			return new Success();
		}

		public async Task<Result> VerifyTwoFactorCode(string emailOrMobile, TwoFactorPurpose purpose, string code, bool setAsUsed, int codeWithinMinutes = 20) {
			if ( emailOrMobile == null ) {
				throw new ArgumentNullException(nameof(emailOrMobile));
			}
			if ( code == null ) {
				throw new ArgumentException(nameof(code));
			}

			var record = _db.TwoFactorCodes
				.Where(x => x.IsUsed == false)
				.Where(x => x.CreatedTime > DateTime.Now.AddMinutes(0 - codeWithinMinutes))
				.Where(x => x.Purpose == purpose)
				.Where(x => x.SentTo == emailOrMobile)
				.Where(x => _my.IsAnonymous || x.UserId == _my.Id<Guid>())
				.OrderByDescending(x => x.CreatedTime)
				.Take(1)
				.FirstOrDefault();

			if ( record == null || string.Compare(code, record.Code, true) != 0 ) {
				return new Failure("验证码不正确或已经过期。".Xslate());
			}
			if ( setAsUsed ) {
				record.IsUsed = true;
				await _db.SaveChangesAsync();
			}
			return new Success();
		}

		private async Task SendViaEmail(string content, string emailAddress) {
			if ( _mailSender == null ) {
				throw new InvalidOperationException("没找到邮件发送服务模块。".Xslate());
			}
			await _mailSender.SendAsync("动态验证码".Xslate(), content, emailAddress);
		}

		private async Task SendViaSms(string content, string mobileNumber) {
			if ( _smsSender == null ) {
				throw new InvalidOperationException("没找到无短信发送服务模块。".Xslate());
			}
			await _smsSender.SendAsync(content, mobileNumber);
		}
	}
}