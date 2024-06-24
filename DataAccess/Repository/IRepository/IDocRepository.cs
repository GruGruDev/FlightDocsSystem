using FlightDocsSystem.Models;

namespace FlightDocsSystem.DataAccess.Repository.IRepository
{
	public interface IDocRepository : IRepository<Doc>
	{
		void Update(Doc doc);
	}
}
