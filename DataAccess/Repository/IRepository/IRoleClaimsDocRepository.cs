using FlightDocsSystem.Models;

namespace FlightDocsSystem.DataAccess.Repository.IRepository
{
	public interface IRoleClaimsDocRepository : IRepository<RoleClaimsDoc>
	{
		void Update(RoleClaimsDoc roleClaimsDoc);
	}
}
