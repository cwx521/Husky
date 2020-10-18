using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public class HuskyInjector
	{
		internal HuskyInjector(IServiceCollection services) {
			Services = services;
		}

		public IServiceCollection Services { get; }
	}
}
