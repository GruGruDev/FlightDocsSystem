using System.ComponentModel.DataAnnotations;

namespace FlightDocsSystem.Models.DTO.Auth
{
	public class LoginRequestDTO
	{
		[Required(ErrorMessage = "Nhập tài khoản")]
		public string Username { get; set; } = string.Empty;
		[Required(ErrorMessage = "Nhập mật khẩu")]
		public string Password { get; set; } = string.Empty;

	}
}
