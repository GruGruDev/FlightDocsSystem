using System.ComponentModel.DataAnnotations;

namespace FlightDocsSystem.Models.DTO.DocType
{
	public class AddOrUpdateDocTypeRequestDTO
	{
        public AddOrUpdateDocTypeRequestDTO()
        {
			RoleAndRoleClaimList = new();
		}

        [Required(ErrorMessage = "Nhập tên loại tài liệu")]
		public string Name { get; set; } = string.Empty;
		public string? Note { get; set; }

		[Required(ErrorMessage = "Nhập tên vai trò")]
		public List<RoleAndRoleClaimList> RoleAndRoleClaimList { get; set; }
	}
}
