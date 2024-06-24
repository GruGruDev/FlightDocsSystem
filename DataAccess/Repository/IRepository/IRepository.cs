using System.Linq.Expressions;


namespace FlightDocsSystem.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        T Find(int id);
        IQueryable<T> GetAll();
		IQueryable<T> Get(Expression<Func<T, bool>> filter, bool asNoTracking);
        void Add(T entity);
		void AddAsync(T entity);
		void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
