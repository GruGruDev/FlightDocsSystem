using FlightDocsSystem.DataAccess.Data;
using FlightDocsSystem.DataAccess.Repository.IRepository;
using FlightDocsSystem.Models;

namespace FlightDocsSystem.DataAccess.Repository
{
	public class AppRoleRepository : Repository<AppRole>, IAppRoleRepository
	{
		public AppRoleRepository(ApplicationDbContext db) : base(db)
		{
		}

		public void Update(AppRole appRole)
		{
			_db.Update(appRole);
		}
	}
}
