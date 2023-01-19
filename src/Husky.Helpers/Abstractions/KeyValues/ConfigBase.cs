using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Husky.KeyValues;
using Microsoft.Extensions.Configuration;

namespace Husky
{
	public abstract partial class ConfigBase
	{
		protected ConfigBase(IKeyValueManager keyValues, IConfiguration? appSettings) {
			KeyValues = keyValues;
			Configuration = appSettings;

			var hasChange = false;
			var props = this.GetType().GetProperties();

			foreach ( var p in props ) {

				if ( p.IsDefined(typeof(NotMappedAttribute), false) ) {
					continue;
				}
				if ( p.SetMethod == null ) {
					continue;
				}

				var defaultValue = p.GetValue(this);
				if ( defaultValue != null ) {
					var dbValue = KeyValues.GetOrAdd(p.Name, defaultValue.ToString());

					if ( !defaultValue.Equals(dbValue) ) {
						p.SetValue(this, Convert.ChangeType(dbValue, p.PropertyType));
						hasChange = true;
					}
				}
			}

			if ( hasChange ) {
				KeyValues.SaveAll();
			}
		}

		protected IKeyValueManager KeyValues { get; }
		protected IConfiguration? Configuration { get; }
		
		public void Save(string key, string value) => KeyValues.Save(key, value);
		public void Save<T>(string key, T value) where T : struct => KeyValues.Save(key, value);
		public void SaveAll() => KeyValues.SaveAll();
		public async Task SaveAllAsync() => await KeyValues.SaveAllAsync();


		[NotMapped] public virtual bool IsTestEnv => Configuration?.GetValue<bool>("IsTestEnv") ?? false;
		[NotMapped] public virtual string? SecretToken => Configuration?.GetValue<string>("Security:SecretToken");
		[NotMapped] public virtual string? SuperCode => Configuration?.GetValue<string>("Security:SuperCode");
	}
}
