using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class HuskyInjectorHelper
	{
		public static HuskyInjector Husky(this IServiceCollection services) => new HuskyInjector(services);
	}
}
