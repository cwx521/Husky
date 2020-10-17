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

		public static HuskyDI AddQQLbs(this HuskyDI husky, QQLbsSettings settings) {
			husky.Services.AddSingleton<ILbs>(new QQLbsService(settings));
			return husky;
		}

		public static HuskyDI AddLbs<TImpelement>(this HuskyDI husky)
			where TImpelement : class, ILbs {
			husky.Services.AddSingleton<ILbs, TImpelement>();
			return husky;
		}

		public static HuskyDI AddLbs<TImpelement>(this HuskyDI husky, Func<IServiceProvider, TImpelement> implementationFactory)
			where TImpelement : class, ILbs {
			husky.Services.AddSingleton<ILbs, TImpelement>(implementationFactory);
			return husky;
		}
	}
}