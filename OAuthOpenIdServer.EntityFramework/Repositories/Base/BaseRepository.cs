using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OAuthOpenIdServer.ApplicationLogic.Boundaries.EntityFramework.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OAuthOpenIdServer.EntityFramework.Repositories.Base
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly ApplicationDatabaseContext _dbContext;

        public BaseRepository(ApplicationDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public T Add(T item)
        {
            AsSet().Add(item);
            return item;
        }

        public IEnumerable<T> AddAll(IEnumerable<T> items)
        {
            AsSet().AddRange(items);

            return items;
        }

        public virtual IEnumerable<T> All()
        {
            return AsSet();
        }

        public async Task<IEnumerable<T>> AllAsync()
        {
            return await AsSet().ToListAsync();
        }

        public Task<long> CountAsync()
        {
            return AsSet().LongCountAsync();
        }

        public Task<long> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return AsSet().LongCountAsync(predicate);
        }

        public void Delete(T item)
        {
            AsSet().Remove(item);
        }

        public void DeleteAll(ISet<T> items)
        {
            AsSet().RemoveRange(items);
        }

        public virtual async Task<T> FindAsync(dynamic id)
        {
            return await AsSet().FindAsync(id);
        }

        public virtual IEnumerable<T> Paginate<TKey>(int pageSize, int pageNumber, Func<T, TKey> keySelector)
        {
            return AsSet().OrderBy(keySelector)
                          .Skip(pageNumber * pageSize)
                          .Take(pageSize);
        }

        public async Task<int> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Update(T item)
        {
            var entry = item as EntityEntry<T>;
            if (entry != null)
            {
                entry.State = EntityState.Modified;
            }
        }

        public virtual IEnumerable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return AsSet().Where(predicate);
        }

        public virtual async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await AsSet().Where(predicate).ToListAsync();
        }

        protected DbSet<T> AsSet()
        {
            return _dbContext.Set<T>();
        }
    }
}