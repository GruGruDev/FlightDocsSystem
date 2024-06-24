using FlightDocsSystem.Models.DTO.Doc;
using FlightDocsSystem.Models.Response;

namespace FlightDocsSystem.Services.Doc.Interfaces
{
	public interface IDocServices
	{
		public Task<ApiResponse<GetDocsResponseDTO>> GetDocs();
		public Task<ApiResponse<Models.Doc>> GetDoc(int docId);
		public Task<ApiResponse<Models.Doc>> DownLoadDoc(int docId);
		public Task<ApiResponse<object>> AddDoc(AddOrUpdateDocRequestDTO model);
		public Task<ApiResponse<object>> UpdateDoc(int docId, AddOrUpdateDocRequestDTO model);
		public Task<ApiResponse<object>> DeleteDoc(int docId);
	}
}
