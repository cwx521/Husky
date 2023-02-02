using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Husky.KeyValues;
using Microsoft.Extensions.Configuration;

namespace Husky
{
	public abstract partial class ConfigBase
	{
		protected ConfigBase(IKeyValueManager keyValues, IConfiguration? appSettings) {
			KeyValues = keyValues;
			Configuration = appSettings;
			Reload();
		}

		protected IKeyValueManager KeyValues { get; }
		protected IConfiguration? Configuration { get; }

		public virtual void Reload() {
			KeyValues.Reload();

			var allKeys = KeyValues.AllKeys;
			var props = this.GetType().GetProperties();

			foreach (var p in props) {
				if (p.SetMethod == null) {
					continue;
				}
				if (p.IsDefined(typeof(NotMappedAttribute), false)) {
					continue;
				}

				if (allKeys.Any(x => x == p.Name)) {
					var dbValue = KeyValues.Get(p.Name);
					p.SetValue(this, Convert.ChangeType(dbValue, p.PropertyType));
				}
			}
		}

		[NotMapped] public virtual bool IsTestEnv => Configuration?.GetValue<bool>("IsTestEnv") ?? false;
		[NotMapped] public virtual string? SecretToken => Configuration?.GetValue<string>("Security:SecretToken");
		[NotMapped] public virtual string? SuperCode => Configuration?.GetValue<string>("Security:SuperCode");
	}
}
