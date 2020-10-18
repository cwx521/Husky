using System;
using Husky.Lbs;
using Husky.Lbs.QQLbs;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyInjector AddQQLbs(this HuskyInjector husky, string key) {
			husky.Services.AddSingleton<ILbs>(new QQLbsService(key));
			return husky;
		}

		public static HuskyInjector AddQQLbs(this HuskyInjector husky, QQLbsSettings options) {
			husky.Services.AddSingleton<ILbs>(new QQLbsService(options));
			return husky;
		}

		public static HuskyInjector AddQQLbs(this HuskyInjector husky, Action<QQLbsSettings> setupAction) {
			var options = new QQLbsSettings();
			setupAction(options);
			return husky.AddQQLbs(options);
		}

		public static HuskyInjector MapLbs<TLbsImpelement>(this HuskyInjector husky)
			where TLbsImpelement : class, ILbs {
			husky.Services.AddSingleton<ILbs, TLbsImpelement>();
			return husky;
		}

		public static HuskyInjector MapLbs<TLbsImpelement>(this HuskyInjector husky, Func<IServiceProvider, TLbsImpelement> implementationFactory)
			where TLbsImpelement : class, ILbs {
			husky.Services.AddSingleton<ILbs, TLbsImpelement>(implementationFactory);
			return husky;
		}
	}
}