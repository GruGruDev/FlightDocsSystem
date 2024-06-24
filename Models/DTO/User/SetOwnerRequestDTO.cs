using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace FlightDocsSystem.Models.DTO.User
{
	public class SetOwnerRequestDTO
	{
		[Required(ErrorMessage = "Chọn ownner")]
		public string UserId { get; set; } = string.Empty;
		[Required(ErrorMessage = "Nhập tài khoản admin")]
		public string UserIdOfAdmin { get; set; } = string.Empty;
		[Required(ErrorMessage ="Nhập mật khẩu")]
		public string Password { get; set; } = string.Empty;
	}
}
