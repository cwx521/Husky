using System;
using Husky.KeyValues;
using Husky.Lbs;
using Husky.Mail;
using Husky.Sms;
using Husky.TwoFactor;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public class HuskyInjector
	{
		internal HuskyInjector(IServiceCollection services) {
			Services = services;
		}

		public IServiceCollection Services { get; }


		public HuskyInjector MapKeyValueManager<TImplement>() where TImplement : class, IKeyValueManager {
			Services.AddScoped<IKeyValueManager, TImplement>();
			return this;
		}
		public HuskyInjector AddKeyValueManager<TImplement>(Func<IServiceProvider, TImplement> implementationFactory) where TImplement : class, IKeyValueManager {
			Services.AddScoped<IKeyValueManager, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector MapLbs<TImplement>() where TImplement : class, ILbs {
			Services.AddScoped<ILbs, TImplement>();
			return this;
		}
		public HuskyInjector AddLbs<TImplement>(Func<IServiceProvider, TImplement> implementationFactory) where TImplement : class, ILbs {
			Services.AddScoped<ILbs, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector MapMailSender<TImplement>() where TImplement : class, IMailSender {
			Services.AddScoped<IMailSender, TImplement>();
			return this;
		}
		public HuskyInjector AddMailSender<TImplement>(Func<IServiceProvider, TImplement> implementationFactory) where TImplement : class, IMailSender {
			Services.AddScoped<IMailSender, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector MapSmsSender<TImplement>() where TImplement : class, ISmsSender {
			Services.AddScoped<ISmsSender, TImplement>();
			return this;
		}
		public HuskyInjector AddSmsSender<TImplement>(Func<IServiceProvider, TImplement> implementationFactory) where TImplement : class, ISmsSender {
			Services.AddScoped<ISmsSender, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector MapTwoFactor<TImplement>() where TImplement : class, ITwoFactorManager {
			Services.AddScoped<ITwoFactorManager, TImplement>();
			return this;
		}
		public HuskyInjector AddTwoFactor<TImplement>(Func<IServiceProvider, TImplement> implementationFactory) where TImplement : class, ITwoFactorManager {
			Services.AddScoped<ITwoFactorManager, TImplement>(implementationFactory);
			return this;
		}
	}
}
