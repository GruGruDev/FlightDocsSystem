using FlightDocsSystem.DataAccess.Data;
using FlightDocsSystem.DataAccess.Repository.IRepository;
using FlightDocsSystem.Models;

namespace FlightDocsSystem.DataAccess.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		public ApplicationDbContext _db;
		public IApplicationUserRepository ApplicationUser { get; private set; }
		public IAppRoleRepository AppRole { get; private set; }
		public IDocRepository Doc { get; private set; }
		public IDocItemRepository DocItem { get; private set; }
		public IDocTypeRepository DocType { get; private set; }
		public IFlightRepository Flight { get; private set; }
		public IRoleClaimsDocRepository RoleClaimsDoc { get; private set; }
		public IRoleClaimsTypeRepository RoleClaimsType { get; private set; }


		public UnitOfWork(ApplicationDbContext db)
		{
			_db = db;
			ApplicationUser = new ApplicationUserRepository(_db);
			AppRole = new AppRoleRepository(_db);
			Doc = new DocRepository(_db);
			DocItem = new DocItemRepository(_db);
			DocType = new DocTypeRepository(_db);
			Flight = new FlightRepository(_db);
			RoleClaimsDoc = new RoleClaimsDocRepository(_db);
			RoleClaimsType = new RoleClaimsTypeRepository(_db);
		}

		public void Save()
		{
			_db.SaveChanges();
		}

		public void SaveAsync()
		{
			_db.SaveChangesAsync();
		}
	}
}
