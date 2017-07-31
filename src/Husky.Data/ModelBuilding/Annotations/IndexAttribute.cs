using System;

namespace Husky.Data.ModelBuilding.Annotations
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class IndexAttribute : Attribute
	{
		public IndexAttribute() {
		}

		public bool IsUnique { get; set; }
		public bool IsClustered { get; set; }
	}
}