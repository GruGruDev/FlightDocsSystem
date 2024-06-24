using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightDocsSystem.Models
{
	public class Doc
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; } = string.Empty;
		[Required]
		public byte[] File { get; set; }
		[Required]
		public string FileExtension { get; set; } = string.Empty;
		public string? Note { get; set; } = string.Empty;

		[Required]
		public int FlightId { get; set; }
		[ForeignKey("FlightId")]
		public Flight Flight { get; set; }
		[Required]
		public int TypeId { get; set; }
		[ForeignKey("TypeId")]
		public DocType Type { get; set; }

		public ICollection<RoleClaimsDoc>? RoleClaimsDocs { get; set; }
		public ICollection<DocItem>? DocItems { get; set; }
	}
}
