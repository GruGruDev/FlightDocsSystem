using FlightDocsSystem.Models.DTO.Auth;
using FlightDocsSystem.Models.Response;

namespace FlightDocsSystem.Services.Auth.Innerfaces
{
	public interface IAuthServices
	{
		public Task<ApiResponse<object>> Login(LoginRequestDTO model);
	}
}
