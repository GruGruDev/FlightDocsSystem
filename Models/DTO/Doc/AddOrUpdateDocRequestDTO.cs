using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FlightDocsSystem.Models.DTO.Doc
{
	public class AddOrUpdateDocRequestDTO
	{
		[Required(ErrorMessage ="Nhập file")]
		public IFormFile File { get; set; }
		[Required(ErrorMessage = "Nhập têm file")]
		public string Name { get; set; } = string.Empty;
		[Required(ErrorMessage = "Nhập loại tài liệu")]
		public int DocTypeId { get; set; }
		[Required(ErrorMessage = "Nhập chuyến bay")]
		public int FlightId { get; set; }
		[Required(ErrorMessage = "Nhập người dùng")]
		public string UserId { get; set; } = string.Empty;
		public string? Note { get; set; }

		[Required(ErrorMessage = "Nhập phân quyền")]
		public List<string>? RoleNameList { get; set; }
	}
}
