using FlightDocsSystem.DataAccess.Data;
using FlightDocsSystem.DataAccess.Repository.IRepository;
using FlightDocsSystem.Models;

namespace FlightDocsSystem.DataAccess.Repository
{
	public class DocItemRepository : Repository<DocItem>, IDocItemRepository
	{
		public DocItemRepository(ApplicationDbContext db) : base(db)
		{
		}

		public void Update(DocItem docItem)
		{
			_db.Update(docItem);
		}
	}
}
