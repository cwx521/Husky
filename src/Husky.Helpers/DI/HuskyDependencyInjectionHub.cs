using Microsoft.Extensions.DependencyInjection;

namespace Husky.DependencyInjection
{
	public class HuskyDependencyInjectionHub
	{
		internal HuskyDependencyInjectionHub(IServiceCollection services) {
			Services = services;
		}
		public IServiceCollection Services { get; private set; }
	}

	public static class HuskyDependencyInjectionHelper
	{
		public static HuskyDependencyInjectionHub Husky(this IServiceCollection services)
			=> new HuskyDependencyInjectionHub(services);
	}
}
