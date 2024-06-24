namespace FlightDocsSystem.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork
	{
		IApplicationUserRepository ApplicationUser { get; }
		IAppRoleRepository AppRole { get; }
		IDocRepository Doc { get; }
		IDocItemRepository DocItem { get; }
		IDocTypeRepository DocType { get; }
		IFlightRepository Flight { get; }
		IRoleClaimsDocRepository RoleClaimsDoc { get; }
		IRoleClaimsTypeRepository RoleClaimsType { get; }
	
		void Save();
		void SaveAsync();
	}
}
