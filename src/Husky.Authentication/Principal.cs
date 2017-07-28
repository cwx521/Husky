using System;
using Husky.Authentication.Abstractions;
using Husky.Sugar;
using Microsoft.AspNetCore.Http;

namespace Husky.Authentication
{
	public class Principal<T> : Identity, IIdentity, IPrincipal where T : IFormattable, IEquatable<T>
	{
		public Principal(IIdentityManager identityManager, IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor) {
			var identity = identityManager.ReadIdentity();
			if ( identity != null && identity.IsAuthenticated ) {
				IdString = identity.IdString;
				DisplayName = identity.DisplayName;
				IdentityManager.SaveIdentity(this);
			}

			this.IdentityManager = identityManager;
			this.ServiceProvider = serviceProvider;
			this.HttpContext = httpContextAccessor.HttpContext;
		}

		public T Id {
			get => IdString.As<T>();
			set => IdString = value?.ToString();
		}
		public IIdentityManager IdentityManager { get; private set; }
		public IServiceProvider ServiceProvider { get; private set; }
		public HttpContext HttpContext { get; private set; }
	}
}