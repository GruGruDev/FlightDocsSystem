using FlightDocsSystem.Models.DTO.Flight;
using FlightDocsSystem.Models.Response;

namespace FlightDocsSystem.Services.Docs.Interface
{
	public interface IFlightService
	{
		public Task<ApiResponse<GetFilghtsResponseDTO>> GetFilghts();
		public Task<ApiResponse<Models.Flight>> GetFilght(int flightId);
		public Task<ApiResponse<GetFilghtsResponseDTO>> SearchFilghts(int? flightId, DateTime? date, int? categoryId);
		public Task<ApiResponse<object>> AddFilght(AddFlightRequestDTO model);
		public Task<ApiResponse<object>> UpdateFilght(int flightId, UpdateFilghtRequestDTO model);
		public Task<ApiResponse<object>> DeleteFilght(int flightId);
	}
}
