using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace FamilyBudget.Www.Models.Repository
{
    public interface IGenericRepository<out TContext, TEntity>
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
