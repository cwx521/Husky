using System;
using Husky.Diagnostics;
using Husky.KeyValues;
using Husky.Lbs;
using Husky.Mail;
using Husky.Principal;
using Husky.Principal.Administration;
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
		public HuskyInjector AddKeyValueManager<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, IKeyValueManager {
			Services.AddScoped<IKeyValueManager, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector MapDiagnosticsLogger<TImplement>() where TImplement : class, IDiagnosticsLogger {
			Services.AddScoped<IDiagnosticsLogger, TImplement>();
			return this;
		}
		public HuskyInjector AddDiagnosticsLogger<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, IDiagnosticsLogger {
			Services.AddScoped<IDiagnosticsLogger, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector MapLbs<TImplement>() where TImplement : class, ILbs {
			Services.AddScoped<ILbs, TImplement>();
			return this;
		}
		public HuskyInjector AddLbs<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, ILbs {
			Services.AddScoped<ILbs, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector MapMailSender<TImplement>() where TImplement : class, IMailSender {
			Services.AddScoped<IMailSender, TImplement>();
			return this;
		}
		public HuskyInjector AddMailSender<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, IMailSender {
			Services.AddScoped<IMailSender, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector MapSmsSender<TImplement>() where TImplement : class, ISmsSender {
			Services.AddScoped<ISmsSender, TImplement>();
			return this;
		}
		public HuskyInjector AddSmsSender<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, ISmsSender {
			Services.AddScoped<ISmsSender, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector MapTwoFactor<TImplement>() where TImplement : class, ITwoFactorManager {
			Services.AddScoped<ITwoFactorManager, TImplement>();
			return this;
		}
		public HuskyInjector AddTwoFactor<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, ITwoFactorManager {
			Services.AddScoped<ITwoFactorManager, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector MapIdentityManager<TImplement>()
			where TImplement : class, IIdentityManager {
			Services.AddScoped<IIdentityManager, TImplement>();
			return this;
		}
		public HuskyInjector AddIdentityManager<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, IIdentityManager {
			Services.AddScoped<IIdentityManager, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector MapPrincipal<TImplement>()
			where TImplement : class, IPrincipalUser {
			Services.AddScoped<IPrincipalUser, TImplement>();
			return this;
		}
		public HuskyInjector AddPrincipal<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, IPrincipalUser {
			Services.AddScoped<IPrincipalUser, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector MapPrincipalAdmin<TImplement>()
			where TImplement : class, IPrincipalAdmin {
			Services.AddScoped<IPrincipalAdmin, TImplement>();
			return this;
		}
		public HuskyInjector AddPrincipalAdmin<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, IPrincipalAdmin {
			Services.AddScoped<IPrincipalAdmin, TImplement>(implementationFactory);
			return this;
		}
	}
}
