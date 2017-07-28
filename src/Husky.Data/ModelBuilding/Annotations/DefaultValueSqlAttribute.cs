using System;

namespace Husky.Data.ModelBuilding.Annotations
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class DefaultValueSqlAttribute : Attribute
	{
		public DefaultValueSqlAttribute(string valueSql) {
			ValueSql = valueSql;
		}

		public string ValueSql { get; set; }
	}
}