using FlightDocsSystem.Models;

namespace FlightDocsSystem.DataAccess.Repository.IRepository
{
	public interface IDocItemRepository : IRepository<DocItem>
	{
		void Update(DocItem docItem);
	}
}
