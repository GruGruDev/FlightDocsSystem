namespace FlightDocsSystem.Models.DTO.Flight
{
	public class GetFilghtsResponseDTO
	{
		public GetFilghtsResponseDTO()
		{
			Flights = new();
		}

		public List<Models.Flight> Flights { get; set; }
	}
}
