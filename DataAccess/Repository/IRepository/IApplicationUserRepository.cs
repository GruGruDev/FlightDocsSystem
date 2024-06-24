using FlightDocsSystem.Models;

namespace FlightDocsSystem.DataAccess.Repository.IRepository
{
	public interface IApplicationUserRepository : IRepository<ApplicationUser>
	{
		void Update(ApplicationUser applicationUser);
	}
}
