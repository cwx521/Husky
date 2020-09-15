using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.KeyValues.Data
{
	public class KeyValue
	{
		[Key, Column(TypeName = "varchar(50)")]
		public string Key { get; set; } = null!;

		[MaxLength(2000)]
		public string? Value { get; set; }
	}
}
