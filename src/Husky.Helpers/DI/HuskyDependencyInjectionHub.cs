using Microsoft.Extensions.DependencyInjection;

namespace Husky.DependencyInjection
{
	public class HuskyDependencyInjectionHub
	{
		internal HuskyDependencyInjectionHub(IServiceCollection serviceColleciton) {
			Services = serviceColleciton;
		}
		public IServiceCollection Services { get; private set; }
	}

	public static class HuskyDependencyInjectionHelper
	{
		public static HuskyDependencyInjectionHub Husky(this IServiceCollection serviceColleciton) => new HuskyDependencyInjectionHub(serviceColleciton);
	}
}
