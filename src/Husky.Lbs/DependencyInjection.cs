using System;
using Husky.Lbs;
using Husky.Lbs.QQLbs;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyServiceHub AddQQLbs(this HuskyServiceHub husky, string key) {
			husky.Services.AddSingleton<ILbs>(new QQLbsService(key));
			return husky;
		}

		public static HuskyServiceHub AddQQLbs(this HuskyServiceHub husky, QQLbsOptions options) {
			husky.Services.AddSingleton<ILbs>(new QQLbsService(options));
			return husky;
		}

		public static HuskyServiceHub AddQQLbs(this HuskyServiceHub husky, Action<QQLbsOptions> setupAction) {
			var options = new QQLbsOptions();
			setupAction(options);
			husky.Services.AddSingleton<ILbs>(new QQLbsService(options));
			return husky;
		}
	}
}