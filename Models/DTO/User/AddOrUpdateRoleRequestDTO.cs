using System.ComponentModel.DataAnnotations;

namespace FlightDocsSystem.Models.DTO.User
{
	public class AddOrUpdateRoleRequestDTO
	{
		[Required(ErrorMessage = "Nhập tên vai trò")]
		public string RoleName { get; set; } = string.Empty;
		public string? Note { get; set; }
	}
}
