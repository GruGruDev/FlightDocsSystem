using FlightDocsSystem.DataAccess.Repository.IRepository;
using FlightDocsSystem.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FlightDocsSystem.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _db;
        public DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

		public void AddAsync(T entity)
		{
			dbSet.AddAsync(entity);
		}

		public T Find(int id)
        {
            return dbSet.Find(id);
        }

		public IQueryable<T> Get(Expression<Func<T, bool>> filter,bool asNoTracking)
		{
			IQueryable<T> query;
			if (asNoTracking)
			{
				query = dbSet.Where(filter).AsNoTracking();
			}
			else
			{
				query = dbSet.Where(filter);
			}
			return query;
		}

        public IQueryable<T> GetAll()
        {
            return dbSet;
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }
    }
}
