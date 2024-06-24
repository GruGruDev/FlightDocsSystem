using FlightDocsSystem.DataAccess.Data;
using FlightDocsSystem.DataAccess.Repository.IRepository;
using FlightDocsSystem.Models;

namespace FlightDocsSystem.DataAccess.Repository
{
	public class DocRepository : Repository<Doc>, IDocRepository
	{
		public DocRepository(ApplicationDbContext db) : base(db)
		{
		}

		public void Update(Doc doc)
		{
			_db.Update(doc);
		}
	}
}
