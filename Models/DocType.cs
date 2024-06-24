using System.ComponentModel.DataAnnotations;

namespace FlightDocsSystem.Models
{
	public class DocType
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; } = string.Empty;
		[Required]
		public string Note { get; set; } = string.Empty;

		public ICollection<Doc>? Docs { get; set; }
		public ICollection<RoleClaimsType>? RoleClaimsTypes { get; set; }
	}
}
