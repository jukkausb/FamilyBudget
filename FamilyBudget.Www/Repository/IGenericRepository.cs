using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FamilyBudget.Www.Repository
{
    public interface IGenericRepository<TContext, TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);
        void Add(TEntity entity);
        void Delete(TEntity entity);
        void Edit(TEntity entity);
        void SaveChanges();
        TContext Context { get; }
    }
}
