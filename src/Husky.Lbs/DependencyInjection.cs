using Husky.Lbs;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddQQLbs(this HuskyDI husky, string key) {
			husky.Services.AddSingleton<ILbs>(new QQLbs(key));
			return husky;
		}

		public static HuskyDI AddQQLbs(this HuskyDI husky, QQLbsSettings settings) {
			husky.Services.AddSingleton<ILbs>(new QQLbs(settings));
			return husky;
		}
	}
}