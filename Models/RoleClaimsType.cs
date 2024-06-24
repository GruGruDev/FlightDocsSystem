using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightDocsSystem.Models
{
	public class RoleClaimsType
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
		public int TypeId { get; set; }
		[ForeignKey("TypeId")]
		public DocType DocType { get; set; }
	}
}
