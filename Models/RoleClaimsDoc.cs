using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlightDocsSystem.Models
{
	public class RoleClaimsDoc
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Type { get; set; } = string.Empty;
		[Required]
		public string Value { get; set; } = string.Empty;

		[Required]
		public string AppRoleId { get; set; }
		[ForeignKey("AppRoleId")]
		public AppRole AppRole { get; set; }
		[Required]
		public int DocsId { get; set; }
		[ForeignKey("DocsId")]
		public Doc Docs { get; set; }
	}
}
