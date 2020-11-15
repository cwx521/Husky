using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public class HuskyServiceHub
	{
		internal HuskyServiceHub(IServiceCollection services) {
			Services = services;
		}

		public IServiceCollection Services { get; }
	}
}
