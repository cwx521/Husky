using System;

namespace Husky
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class DefaultValueSqlAttribute : Attribute
	{
		public DefaultValueSqlAttribute(string sql) {
			Sql = sql;
		}

		public string Sql { get; init; } = null!;
	}
}
