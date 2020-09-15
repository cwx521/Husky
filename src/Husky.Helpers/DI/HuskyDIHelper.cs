using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class HuskyDIHelper
	{
		public static HuskyDI Husky(this IServiceCollection services) => new HuskyDI(services);
	}
}
