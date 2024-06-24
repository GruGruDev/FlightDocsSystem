using FlightDocsSystem.DataAccess.Data;
using FlightDocsSystem.DataAccess.Repository.IRepository;
using FlightDocsSystem.Models;

namespace FlightDocsSystem.DataAccess.Repository
{
	public class FlightRepository : Repository<Flight>, IFlightRepository
	{
		public FlightRepository(ApplicationDbContext db) : base(db)
		{
		}

		public void Update(Flight flight)
		{
			_db.Update(flight);
		}
	}
}
