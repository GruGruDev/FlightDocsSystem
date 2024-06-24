using FlightDocsSystem.Models.DTO.User;
using FlightDocsSystem.Services.User.Interfaces;
using FlightDocsSystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightDocsSystem.Controllers.User
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = SD.Role_Admin)]
	public class UserController : ControllerBase
	{
		private readonly IUserServices _user;

		public UserController(IUserServices user)
		{
			_user = user;
		}

		[HttpGet("GetUsers")]
		public async Task<IActionResult> GetUsers()
		{
			var result = await _user.GetUsersAsync();

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpGet("GetUser/{id}")]
		public async Task<IActionResult> GetUser(string id)
		{
			var result = await _user.GetUserAsync(id);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpPost("AddUser")]
		public async Task<IActionResult> AddUser([FromForm] AddUserResquestDTO model)
		{
			var result = await _user.CreateUserAsync(model);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpPut("UpdateUser/{id}")]
		public async Task<IActionResult> UpdateUser(string id, [FromForm] UpdateUserRequestDTO model)
		{
			var result = await _user.UpdateUserAsync(id, model);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpDelete("DeleteUser/{id}")]
		public async Task<IActionResult> DeleteUser(string id)
		{
			var result = await _user.DeleteUserAsync(id);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		//role
		[HttpGet("GetRoles")]
		public async Task<IActionResult> GetRoles()
		{
			var result = await _user.GetRolesAsync();

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpGet("GetRole/{id}")]
		public async Task<IActionResult> GetRole(string id)
		{
			var result = await _user.GetRoleAsync(id);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpPost("AddRole")]
		public async Task<IActionResult> AddRole([FromForm] AddOrUpdateRoleRequestDTO model)
		{
			var result = await _user.CreateRoleAsync(model);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpPut("UpdateRole/{id}")]
		public async Task<IActionResult> UpdateRole(string id, [FromForm] AddOrUpdateRoleRequestDTO model)
		{
			var result = await _user.UpdateRoleAsync(id, model);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpDelete("DeleteRole/{id}")]
		public async Task<IActionResult> DeleteRole(string id)
		{
			var result = await _user.DeleteRoleAsync(id);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}


		//lock unlock

		[HttpPost("LockUser")]
		public async Task<IActionResult> LockUser([FromForm] string id)
		{
			var result = await _user.LockUserAsync(id);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpPost("LockUsers")]
		public async Task<IActionResult> LockUsers([FromForm] LockOrUnlockUsersRequestDTO model)
		{
			var result = await _user.LockUsersAsync(model);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpPost("UnLockUser")]
		public async Task<IActionResult> UnLockUser([FromForm] string id)
		{
			var result = await _user.UnLockUserAsync(id);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpPost("UnLockUsers")]
		public async Task<IActionResult> UnLockUsers([FromForm] LockOrUnlockUsersRequestDTO model)
		{
			var result = await _user.UnLockUsersAsync(model);

			if (result.IsSuccess)
			{
				return Ok(result);
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpPost("SetOwner")]
		public async Task<IActionResult> SetOwner([FromForm] SetOwnerRequestDTO model)
		{
			var result = await _user.SetOwnerAsync(model);

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
