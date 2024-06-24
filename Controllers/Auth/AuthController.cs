using FlightDocsSystem.Models.DTO.Auth;
using FlightDocsSystem.Services.Auth.Innerfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlightDocsSystem.Controllers.Auth
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthServices _auth;

		public AuthController(IAuthServices auth)
		{
			_auth = auth;
		}

		[HttpPost("Login")]
		public async Task<IActionResult> Login([FromForm] LoginRequestDTO model)
		{
			var result = await _auth.Login(model);

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
