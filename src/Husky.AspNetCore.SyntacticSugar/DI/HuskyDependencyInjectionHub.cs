using Microsoft.Extensions.DependencyInjection;

namespace Husky.AspNetCore.DependencyInjection
{
	public class HuskyDependencyInjectionHub
	{
		internal HuskyDependencyInjectionHub(IServiceCollection serviceColleciton) {
			this.ServiceCollection = serviceColleciton;
		}
		public IServiceCollection ServiceCollection { get; private set; }
	}

	public static class HuskyDependencyInjectionHelper
	{
		public static HuskyDependencyInjectionHub Husky(this IServiceCollection serviceColleciton) {
			return new HuskyDependencyInjectionHub(serviceColleciton);
		}
	}
}
