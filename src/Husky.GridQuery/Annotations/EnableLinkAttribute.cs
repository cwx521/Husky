using System;

namespace Husky.GridQuery
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class EnableLinkAttribute : Attribute
	{
		public EnableLinkAttribute() {
		}

		public EnableLinkAttribute(string linkUrl) {
			LinkUrl = linkUrl;
		}

		public string LinkUrl { get; set; } = null!;
		public GridColumLinkTarget LinkTarget { get; set; }
	}
}
