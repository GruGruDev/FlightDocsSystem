using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightDocsSystem.Models
{
	public class Flight
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string FlightNo { get; set; } = string.Empty;
		[Required]
		public string Route { get; set; } = string.Empty;
		[Required]
		public DateTime Date { get; set; }
		[Required]
		public string PointOfLoading { get; set; } = string.Empty;
		[Required]
		public string PointOfUnLoading { get; set; } = string.Empty;

		public ICollection<Doc>? Docs { get; set; }
	}
}
