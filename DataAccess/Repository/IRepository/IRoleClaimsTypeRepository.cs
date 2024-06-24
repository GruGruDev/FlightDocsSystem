using FlightDocsSystem.Models;

namespace FlightDocsSystem.DataAccess.Repository.IRepository
{
	public interface IRoleClaimsTypeRepository : IRepository<RoleClaimsType>
	{
		void Update(RoleClaimsType roleClaimsType);
	}
}
