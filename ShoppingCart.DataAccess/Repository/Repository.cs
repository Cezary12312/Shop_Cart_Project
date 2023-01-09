using Microsoft.EntityFrameworkCore;
using ShoppingCart.DataAccess.Data;
using System.Linq.Expressions;

namespace ShoppingCart.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly CartDbContext context;
        private DbSet<T> set;

        public Repository(CartDbContext context)
        {
            this.context = context;
            set = context.Set<T>();
        }

        public void Add(T entity) => set.Add(entity);

        public void Remove(T entity) => set.Remove(entity);

        public void RemoveRange(IEnumerable<T> entities) => set.RemoveRange(entities);

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, string? includeProperties = null)
        {
            IQueryable<T> query = set;
            if(predicate != null)
                query = query.Where(predicate);
            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(item);
            }
            return query.ToList();
        }

        public T GetT(Expression<Func<T, bool>>? predicate = null, string? includeProperties = null)
        {
            IQueryable<T> query = set;
            query = query.Where(predicate);
            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(item);
            }
            return query.FirstOrDefault();
        }
    }
}
