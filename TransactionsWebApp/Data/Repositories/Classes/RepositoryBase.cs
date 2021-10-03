using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TransactionsWebApp.Data.Repositories
{
    public abstract class RepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class
        where TKey : struct
    {
        protected ApplicationDbContext Context { get; private set; }

        protected RepositoryBase(ApplicationDbContext context)
        {
            Context = context;
        }

        public TEntity Create(TEntity newEntity)
        {
            Context.Set<TEntity>().Add(newEntity);
            Context.SaveChanges();

            return newEntity;
        }

        public TEntity Update(object id, TEntity modifiedEntity)
        {
            var existing = Context.Set<TEntity>().Find(id);

            if (existing == null)
            {
                return null;
            }

            Context.Entry(existing).CurrentValues.SetValues(modifiedEntity);
            Context.SaveChanges();

            return modifiedEntity;
        }

        /// <summary>
        /// Returns an entity if id is found. Otherwise returns null
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity Retrieve(object id)
        {
            return Context.Set<TEntity>().Find(id);
        }

        public virtual IList<TEntity> Retrieve(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TKey>> sort, int skip, int top)
        {
            return Context.Set<TEntity>()
                          .Where(filter)
                          .OrderBy(sort)
                          .Skip(skip)
                          .Take(top)
                          .ToList();
        }

        public virtual IList<TEntity> RetrieveAll(Expression<Func<TEntity, bool>> filter)
        {
            return Context.Set<TEntity>()
                          .Where(filter)
                          .ToList();
        }

        public virtual IList<TEntity> RetrieveAll()
        {
            return Context.Set<TEntity>()
                .Select(x => x)
                .ToList();
        }

        public virtual IQueryable<TEntity> RetrieveLazy(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TKey>> sort, int skip, int top)
        {
            return Context.Set<TEntity>()
                          .Where(filter)
                          .OrderBy(sort)
                          .Skip(skip)
                          .Take(top)
                          .Select(x => x);
        }

        public virtual IQueryable<TEntity> RetrieveAllLazy(Expression<Func<TEntity, bool>> filter)
        {
            return Context.Set<TEntity>()
                          .Where(filter)
                          .Select(x => x);
        }

        public virtual IQueryable<TEntity> RetrieveAllLazy()
        {
            return Context.Set<TEntity>()
                .Select(x => x);
        }

        public virtual TEntity RetrieveFirstOrDefault(Expression<Func<TEntity, bool>> filter)
        {
            return Context.Set<TEntity>()
                .Where(filter)
                .Select(x => x)
                .FirstOrDefault();
        }

    }
}