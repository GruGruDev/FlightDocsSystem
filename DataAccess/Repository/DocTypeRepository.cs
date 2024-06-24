using FlightDocsSystem.DataAccess.Data;
using FlightDocsSystem.DataAccess.Repository.IRepository;
using FlightDocsSystem.Models;

namespace FlightDocsSystem.DataAccess.Repository
{
	public class DocTypeRepository : Repository<DocType>, IDocTypeRepository
	{
		public DocTypeRepository(ApplicationDbContext db) : base(db)
		{
		}

		public void Update(DocType docType)
		{
			_db.Update(docType);
		}
	}
}
