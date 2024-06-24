using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightDocsSystem.Models
{
	public class DocItem
	{
		[Key]
		public int Id { get; set; }
		
		[Required]
		public double Version { get; set; }
		public DateTime CreateDate { get; set; } = DateTime.Now;
		[Required]
		public int DocId { get; set; }
		[ForeignKey("DocId")]
		public Doc Doc { get; set; }
		[Required]
		public string UserId { get; set; }
		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; }
	}
}
