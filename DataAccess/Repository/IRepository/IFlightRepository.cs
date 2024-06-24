using FlightDocsSystem.Models;

namespace FlightDocsSystem.DataAccess.Repository.IRepository
{
	public interface IFlightRepository : IRepository<Flight>
	{
		void Update(Flight flight);
	}
}
