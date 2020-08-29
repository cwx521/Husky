using System.ComponentModel.DataAnnotations.Schema;
using Husky.KeyValues;
using Microsoft.Extensions.Configuration;

namespace Shipping.Kernel
{
	public abstract partial class ConfigBase
	{
		protected ConfigBase(KeyValueManager keyValues, IConfiguration appSettings) {
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
				var dbValue = _keyValues.GetOrAdd(p.Name, defaultValue, p.PropertyType);

				if ( !defaultValue.Equals(dbValue) ) {
					p.SetValue(this, dbValue);
					hasChange = true;
				}
			}

			if ( hasChange ) {
				_keyValues.SaveChanges();
			}
		}

		private readonly KeyValueManager _keyValues;
		private readonly IConfiguration _appSettings;

		public void Reload() => _keyValues.Reload();
		public void SaveChanges() => _keyValues.SaveChanges();

		[NotMapped] public virtual bool IsTestEnv => _appSettings.GetValue<bool>("IsTestEnv");
		[NotMapped] public virtual string PermanentToken => _appSettings.GetValue<string>("Security:PermanentToken");
		[NotMapped] public virtual string RerollableToken => _appSettings.GetValue<string>("Security:RerollableToken");
		[NotMapped] public virtual string SuperCode => _appSettings.GetValue<string>("Security:SuperCode");
	}
}
