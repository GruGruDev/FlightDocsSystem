using FlightDocsSystem.Models.DTO.DocType;
using FlightDocsSystem.Models.Response;

namespace FlightDocsSystem.Services.DocType.Interfaces
{
	public interface IDocTypeServices
	{
		public Task<ApiResponse<GetDocTypesResponseDTO>> GetDocTypes();
		public Task<ApiResponse<Models.DocType>> GetDocType(int docTypeId);
		public Task<ApiResponse<object>> AddDocType(AddOrUpdateDocTypeRequestDTO model);
		public Task<ApiResponse<object>> UpdateDocType(int docTypeId, AddOrUpdateDocTypeRequestDTO model);
		public Task<ApiResponse<object>> DeleteDocType(int docTypeId);
	}
}
