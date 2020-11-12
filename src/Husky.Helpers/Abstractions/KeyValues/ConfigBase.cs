using System;
using System.ComponentModel.DataAnnotations.Schema;
using Husky.KeyValues;
using Microsoft.Extensions.Configuration;

namespace Husky
{
	public abstract partial class ConfigBase
	{
		protected ConfigBase(IKeyValueManager keyValues, IConfiguration? appSettings) {
			_keyValues = keyValues;
			_appSettings = appSettings;

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
					var dbValue = _keyValues.GetOrAdd(p.Name, defaultValue.ToString());

					if ( !defaultValue.Equals(dbValue) ) {
						p.SetValue(this, dbValue);
						hasChange = true;
					}
				}
			}

			if ( hasChange ) {
				_keyValues.SaveAll();
			}
		}

		private readonly IKeyValueManager _keyValues;
		private readonly IConfiguration? _appSettings;


		public void Reload() => _keyValues.Reload();
		public void Save(string key, string value) => _keyValues.Save(key, value);
		public void Save<T>(string key, T value) where T : struct, IConvertible => _keyValues.Save(key, value);
		public void SaveAll() => _keyValues.SaveAll();


		[NotMapped] public virtual bool IsTestEnv => _appSettings?.GetValue<bool>("IsTestEnv") ?? false;
		[NotMapped] public virtual string? PermanentToken => _appSettings?.GetValue<string>("Security:PermanentToken");
		[NotMapped] public virtual string? RerollableToken => _appSettings?.GetValue<string>("Security:RerollableToken");
		[NotMapped] public virtual string? SuperCode => _appSettings?.GetValue<string>("Security:SuperCode");
	}
}
