using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public class HuskyDI
	{
		internal HuskyDI(IServiceCollection services) {
			Services = services;
		}

		public IServiceCollection Services { get; private set; }
	}
}
