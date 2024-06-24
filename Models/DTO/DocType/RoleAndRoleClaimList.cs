using System.ComponentModel.DataAnnotations;

namespace FlightDocsSystem.Models.DTO.DocType
{
	public class RoleAndRoleClaimList
	{
		[Required(ErrorMessage = "Nhập tên vai trò")]
		public string RoleName { get; set; } = string.Empty;
		[Required(ErrorMessage = "Nhập quyền")]
		public string value { get; set; } = string.Empty;
	}
}
