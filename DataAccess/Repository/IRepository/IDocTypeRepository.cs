using FlightDocsSystem.Models;

namespace FlightDocsSystem.DataAccess.Repository.IRepository
{
	public interface IDocTypeRepository : IRepository<DocType>
	{
		void Update(DocType docType);
	}
}
