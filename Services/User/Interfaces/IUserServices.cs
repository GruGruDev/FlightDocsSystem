using FlightDocsSystem.Models.DTO.User;
using FlightDocsSystem.Models.Response;

namespace FlightDocsSystem.Services.User.Interfaces
{
	public interface IUserServices
	{
		public Task<ApiResponse<GetUsersResponseDTO>> GetUsersAsync();
		public Task<ApiResponse<Models.ApplicationUser>> GetUserAsync(string userId);
		public Task<ApiResponse<object>> CreateUserAsync(AddUserResquestDTO model);
		public Task<ApiResponse<object>> UpdateUserAsync(string userId, UpdateUserRequestDTO model);
		public Task<ApiResponse<object>> DeleteUserAsync(string userId);

		//role
		public Task<ApiResponse<GetRolesResponseDTO>> GetRolesAsync();
		public Task<ApiResponse<Models.AppRole>> GetRoleAsync(string roleId);
		public Task<ApiResponse<object>> CreateRoleAsync(AddOrUpdateRoleRequestDTO model);
		public Task<ApiResponse<object>> UpdateRoleAsync(string roleId, AddOrUpdateRoleRequestDTO model);
		public Task<ApiResponse<object>> DeleteRoleAsync(string roleId);

		//lock unlock
		public Task<ApiResponse<object>> LockUserAsync(string userId);
		public Task<ApiResponse<object>> LockUsersAsync(LockOrUnlockUsersRequestDTO model);
		public Task<ApiResponse<object>> UnLockUserAsync(string userId);
		public Task<ApiResponse<object>> UnLockUsersAsync(LockOrUnlockUsersRequestDTO model);

		// set owner
		public Task<ApiResponse<object>> SetOwnerAsync(SetOwnerRequestDTO model);

	}
}
