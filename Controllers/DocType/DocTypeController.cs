using FlightDocsSystem.Models.DTO.DocType;
using FlightDocsSystem.Services.DocType.Interfaces;
using FlightDocsSystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightDocsSystem.Controllers.DocType
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = SD.Role_Admin)]
	public class DocTypeController : ControllerBase
	{
		private readonly IDocTypeServices _docType;

		public DocTypeController(IDocTypeServices docType)
		{
			_docType = docType;
		}

		[HttpGet("GetDocTypes")]
		public async Task<IActionResult> GetDocTypes()
		{
			var result = await _docType.GetDocTypes();

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpGet("GetDocType/{id}")]
		public async Task<IActionResult> GetDocType(int id)
		{
			var result = await _docType.GetDocType(id);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpPost("AddDocType")]
		public async Task<IActionResult> AddDocType([FromBody] AddOrUpdateDocTypeRequestDTO model)
		{
			var result = await _docType.AddDocType(model);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpPut("UpdateDocType/{id}")]
		public async Task<IActionResult> UpdateDocType(int id, [FromBody] AddOrUpdateDocTypeRequestDTO model)
		{
			var result = await _docType.UpdateDocType(id, model);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpDelete("DeleteDocType/{id}")]
		public async Task<IActionResult> DeleteDocType(int id)
		{
			var result = await _docType.DeleteDocType(id);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}
	}
}
