using System.ComponentModel.DataAnnotations;

namespace FlightDocsSystem.Models.DTO.Auth
{
	public class ForgotPasswordRequestDTO
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
