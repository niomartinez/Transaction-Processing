using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TransactionsWebApp.Data.Repositories
{/// <summary>
 /// 
 /// </summary>
 /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity, TKey>
        where TEntity : class
        where TKey : struct
    {
        TEntity Create(TEntity newEntity);

        TEntity Update(object id, TEntity modifiedEntity);

        TEntity Retrieve(object id);

        IList<TEntity> Retrieve(Expression<Func<TEntity, Boolean>> filter, Expression<Func<TEntity, TKey>> sort, int skip, int top);

        IList<TEntity> RetrieveAll(Expression<Func<TEntity, Boolean>> filter);

        IList<TEntity> RetrieveAll();

        IQueryable<TEntity> RetrieveLazy(Expression<Func<TEntity, Boolean>> filter, Expression<Func<TEntity, TKey>> sort, int skip, int top);

        IQueryable<TEntity> RetrieveAllLazy(Expression<Func<TEntity, Boolean>> filter);

        IQueryable<TEntity> RetrieveAllLazy();

        TEntity RetrieveFirstOrDefault(Expression<Func<TEntity, bool>> filter);

    }
}