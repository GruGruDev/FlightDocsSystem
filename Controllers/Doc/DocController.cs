using FlightDocsSystem.Attributes;
using FlightDocsSystem.Models.DTO.Doc;
using FlightDocsSystem.Services.Doc.Interfaces;
using FlightDocsSystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightDocsSystem.Controllers.Doc
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class DocController : ControllerBase
	{
		private readonly IDocServices _doc;

		public DocController(IDocServices doc)
		{
			_doc = doc;
		}

		[HttpGet("GetDocs")]
		public async Task<IActionResult> GetDocs()
		{
			var result = await _doc.GetDocs();

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[AuthorizeClaim(SD.Claim_Read)]
		[HttpGet("GetDoc/{id}")]
		public async Task<IActionResult> GetDoc(int id)
		{
			var result = await _doc.GetDoc(id);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpPost("AddDoc")]
		public async Task<IActionResult> AddDoc([FromForm] AddOrUpdateDocRequestDTO model)
		{
			var result = await _doc.AddDoc(model);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[AuthorizeClaim(SD.Claim_Modify)]
		[HttpPut("UpdateDoc/{id}")]
		public async Task<IActionResult> UpdateDoc(int id, [FromForm] AddOrUpdateDocRequestDTO model)
		{
			var result = await _doc.UpdateDoc(id, model);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[AuthorizeClaim(SD.Claim_Modify)]
		[HttpDelete("DeleteDoc/{id}")]
		public async Task<IActionResult> DeleteDoc(int id)
		{
			var result = await _doc.DeleteDoc(id);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpGet("DownloadDoc/{id}")]
		public async Task<IActionResult> DownloadDoc(int id)
		{
			var result = await _doc.DownLoadDoc(id);

			if (result.IsSuccess)
			{
				// Trả về nội dung của tệp dưới dạng FileContentResult
				var fileBytes = result.Result.File;
				var contentType = "application/octet-stream"; // Hoặc có thể sử dụng: "application/pdf" cho file PDF
				var fileName = $"Document_{result.Result.Name}"+$"{result.Result.FileExtension}"; // Đặt tên cho tệp tải xuống

				return File(fileBytes, contentType, fileName);
			}
			else
			{
				return BadRequest(result);
			}
		}
	}
}
