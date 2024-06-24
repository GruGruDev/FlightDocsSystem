namespace FlightDocsSystem.Models.DTO.Doc
{
	public class GetDocsResponseDTO
	{
		public GetDocsResponseDTO()
		{
			Docs = new();
		}
		public List<Models.Doc> Docs { get; set; }
	}
}
