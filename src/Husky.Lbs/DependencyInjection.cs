using System;
using Husky.Lbs;
using Husky.Lbs.QQLbs;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddQQLbs(this HuskyDI husky, string key) {
			husky.Services.AddSingleton<ILbs>(new QQLbsService(key));
			return husky;
		}

		public static HuskyDI AddQQLbs(this HuskyDI husky, QQLbsSettings options) {
			husky.Services.AddSingleton<ILbs>(new QQLbsService(options));
			return husky;
		}

		public static HuskyDI AddQQLbs(this HuskyDI husky, Action<QQLbsSettings> setupAction) {
			var options = new QQLbsSettings();
			setupAction(options);
			return husky.AddQQLbs(options);
		}

		public static HuskyDI MapLbs<TLbsImpelement>(this HuskyDI husky)
			where TLbsImpelement : class, ILbs {
			husky.Services.AddSingleton<ILbs, TLbsImpelement>();
			return husky;
		}

		public static HuskyDI MapLbs<TLbsImpelement>(this HuskyDI husky, Func<IServiceProvider, TLbsImpelement> implementationFactory)
			where TLbsImpelement : class, ILbs {
			husky.Services.AddSingleton<ILbs, TLbsImpelement>(implementationFactory);
			return husky;
		}
	}
}