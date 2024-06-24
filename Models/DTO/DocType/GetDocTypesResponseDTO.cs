namespace FlightDocsSystem.Models.DTO.DocType
{
	public class GetDocTypesResponseDTO
	{
        public GetDocTypesResponseDTO()
        {
			DocTypes = new();
		}
        public List<Models.DocType> DocTypes { get; set; }
    }
}
