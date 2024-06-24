using FlightDocsSystem.Models.DTO.Auth;
using FlightDocsSystem.Models.Response;

namespace FlightDocsSystem.Services.Auth.Interfaces
{
	public interface IForgotPasswordService
	{
		Task<ApiResponse<object>> ForgotPassword(ForgotPasswordRequestDTO model);
		Task<ApiResponse<object>> ChangePassword(ChangePasswordRequestDTO model);
	}
}
