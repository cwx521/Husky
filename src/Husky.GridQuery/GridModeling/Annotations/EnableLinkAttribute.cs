using System;

namespace Husky.GridQuery.GridModeling.Annotations
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
