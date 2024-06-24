using FlightDocsSystem.Models;

namespace FlightDocsSystem.DataAccess.Repository.IRepository
{
	public interface IAppRoleRepository : IRepository<AppRole>
	{
		void Update(AppRole appRole);
	}
}
