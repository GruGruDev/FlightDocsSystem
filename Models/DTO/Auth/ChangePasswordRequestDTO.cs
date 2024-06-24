using System.ComponentModel.DataAnnotations;

namespace FlightDocsSystem.Models.DTO.Auth
{
	public class ChangePasswordRequestDTO
	{
		[Required]
		public string UserId { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		public string ResetToken { get; set; }
	}
}
