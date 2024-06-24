namespace FlightDocsSystem.Models.DTO.User
{
	public class GetRolesResponseDTO
	{
		public GetRolesResponseDTO()
		{
			Roles = new();
		}
		public List<AppRole> Roles { get; set; }
	}
}
