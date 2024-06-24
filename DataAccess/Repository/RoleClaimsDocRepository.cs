using FlightDocsSystem.DataAccess.Data;
using FlightDocsSystem.DataAccess.Repository.IRepository;
using FlightDocsSystem.Models;

namespace FlightDocsSystem.DataAccess.Repository
{
	public class RoleClaimsDocRepository : Repository<RoleClaimsDoc>, IRoleClaimsDocRepository
	{
		public RoleClaimsDocRepository(ApplicationDbContext db) : base(db)
		{
		}

		public void Update(RoleClaimsDoc roleClaimsDoc)
		{
			_db.Update(roleClaimsDoc);
		}
	}
}
