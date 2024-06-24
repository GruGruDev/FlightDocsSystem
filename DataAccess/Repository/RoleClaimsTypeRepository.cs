using FlightDocsSystem.DataAccess.Data;
using FlightDocsSystem.DataAccess.Repository.IRepository;
using FlightDocsSystem.Models;

namespace FlightDocsSystem.DataAccess.Repository
{
	public class RoleClaimsTypeRepository : Repository<RoleClaimsType>, IRoleClaimsTypeRepository
	{
		public RoleClaimsTypeRepository(ApplicationDbContext db) : base(db)
		{
		}

		public void Update(RoleClaimsType roleClaimsType)
		{
			_db.Update(roleClaimsType);
		}
	}
}
