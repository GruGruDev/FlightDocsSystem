namespace FlightDocsSystem.Models.DTO.User
{
	public class LockOrUnlockUsersRequestDTO
	{
		public LockOrUnlockUsersRequestDTO()
		{
			UserIdList = new();
		}

		public List<string> UserIdList { get; set; }
	}
}
