using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class HuskyServiceHelper
	{
		public static HuskyServiceHub Husky(this IServiceCollection services) => new(services);
	}
}
