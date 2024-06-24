using System.ComponentModel.DataAnnotations;

namespace FlightDocsSystem.Models.DTO.Flight
{
	public class UpdateFilghtRequestDTO
	{
		[Required(ErrorMessage = "Nhập mã số chuyến bay")]
		public string FlightNo { get; set; } = string.Empty;
		[Required(ErrorMessage = "Nhập ngày chuyến bay")]
		public DateTime FlightDay { get; set; }
		[Required(ErrorMessage = "Nhập tuyến bay")]
		public string Route { get; set; } = string.Empty;
		[Required(ErrorMessage = "Nhập điểm chất hàng")]
		public string PointOfLoading { get; set; } = string.Empty;
		[Required(ErrorMessage = "Nhập điểm dỡ hàng")]
		public string PointOfUnLoading { get; set; } = string.Empty;
	}
}
