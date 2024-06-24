using Microsoft.AspNetCore.Identity;

namespace FlightDocsSystem.Models
{
	public class AppRole : IdentityRole
	{
		public string? Note {  get; set; }
		public ICollection<RoleClaimsType>? RoleClaimsTypes { get; set; }
		public ICollection<RoleClaimsDoc>? RoleClaimsDocs { get; set; }
	}
}
