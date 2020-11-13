using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.KeyValues.Data
{
	public class KeyValue
	{
		[Key, StringLength(50), Column(TypeName = "varchar(50)"), Required]
		public string Key { get; init; } = null!;

		[StringLength(2000)]
		public string? Value { get; set; }
	}
}
