using System;
using Husky.Diagnostics;
using Husky.FileStore;
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


		public HuskyInjector AddExternalKeyValueManager<TImplement>() where TImplement : class, IKeyValueManager {
			Services.AddScoped<IKeyValueManager, TImplement>();
			return this;
		}
		public HuskyInjector AddExternalKeyValueManager<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, IKeyValueManager {
			Services.AddScoped<IKeyValueManager, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector AddExternalDiagnosticsLogger<TImplement>() where TImplement : class, IDiagnosticsLogger {
			Services.AddScoped<IDiagnosticsLogger, TImplement>();
			return this;
		}
		public HuskyInjector AddExternalDiagnosticsLogger<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, IDiagnosticsLogger {
			Services.AddScoped<IDiagnosticsLogger, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector AddExternalFileStoreLogger<TImplement>()
			where TImplement : class, IFileStoreLogger {
			Services.AddScoped<IFileStoreLogger, TImplement>();
			return this;
		}
		public HuskyInjector AddExternalFileStoreLogger<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, IFileStoreLogger {
			Services.AddScoped<IFileStoreLogger, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector AddExternalLbs<TImplement>() where TImplement : class, ILbs {
			Services.AddScoped<ILbs, TImplement>();
			return this;
		}
		public HuskyInjector AddExternalLbs<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, ILbs {
			Services.AddScoped<ILbs, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector AddExternalMailSender<TImplement>() where TImplement : class, IMailSender {
			Services.AddScoped<IMailSender, TImplement>();
			return this;
		}
		public HuskyInjector AddExternalMailSender<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, IMailSender {
			Services.AddScoped<IMailSender, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector AddExternalSmsSender<TImplement>() where TImplement : class, ISmsSender {
			Services.AddScoped<ISmsSender, TImplement>();
			return this;
		}
		public HuskyInjector AddExternalSmsSender<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, ISmsSender {
			Services.AddScoped<ISmsSender, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector AddExternalTwoFactor<TImplement>() where TImplement : class, ITwoFactorManager {
			Services.AddScoped<ITwoFactorManager, TImplement>();
			return this;
		}
		public HuskyInjector AddExternalTwoFactor<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, ITwoFactorManager {
			Services.AddScoped<ITwoFactorManager, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector AddExternalIdentityManager<TImplement>()
			where TImplement : class, IIdentityManager {
			Services.AddScoped<IIdentityManager, TImplement>();
			return this;
		}
		public HuskyInjector AddExternalIdentityManager<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, IIdentityManager {
			Services.AddScoped<IIdentityManager, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector AddExternalPrincipal<TImplement>()
			where TImplement : class, IPrincipalUser {
			Services.AddScoped<IPrincipalUser, TImplement>();
			return this;
		}
		public HuskyInjector AddExternalPrincipal<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, IPrincipalUser {
			Services.AddScoped<IPrincipalUser, TImplement>(implementationFactory);
			return this;
		}


		public HuskyInjector AddExternalPrincipalAdmin<TImplement>()
			where TImplement : class, IPrincipalAdmin {
			Services.AddScoped<IPrincipalAdmin, TImplement>();
			return this;
		}
		public HuskyInjector AddExternalPrincipalAdmin<TImplement>(Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, IPrincipalAdmin {
			Services.AddScoped<IPrincipalAdmin, TImplement>(implementationFactory);
			return this;
		}
	}
}
