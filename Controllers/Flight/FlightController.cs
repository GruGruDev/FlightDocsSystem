using FlightDocsSystem.Models.DTO.Flight;
using FlightDocsSystem.Services.Docs.Interface;
using FlightDocsSystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightDocsSystem.Controllers.Flight
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = SD.Role_Admin)]
	public class FlightController : ControllerBase
	{
		private readonly IFlightService _flightService;

		public FlightController(IFlightService flightService)
		{
			_flightService = flightService;
		}

		[HttpGet("GetFlights")]
		public async Task<IActionResult> GetFlights()
		{
			var result = await _flightService.GetFilghts();

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpGet("GetFlight/{id}")]
		public async Task<IActionResult> GetFlight(int id)
		{
			var result = await _flightService.GetFilght(id);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpPost("SearchFlights")]
		public async Task<IActionResult> SearchFlights([FromForm] int? id, [FromForm] DateTime? date, [FromForm] int? categoryId)
		{
			var result = await _flightService.SearchFilghts(id, date, categoryId);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpPost("AddFlight")]
		public async Task<IActionResult> AddFlight([FromForm] AddFlightRequestDTO model)
		{
			var result = await _flightService.AddFilght(model);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpPut("UpdateFlight/{id}")]
		public async Task<IActionResult> UpdateFlight(int id, [FromForm] UpdateFilghtRequestDTO model)
		{
			var result = await _flightService.UpdateFilght(id, model);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpDelete("DeleteFlight/{id}")]
		public async Task<IActionResult> DeleteFlight(int id)
		{
			var result = await _flightService.DeleteFilght(id);

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
