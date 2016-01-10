using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace FamilyBudget.Www.Models.Repository
{
    public abstract class GenericRepository<TContext, TEntity> : IGenericRepository<TContext, TEntity>
        where TEntity : class
        where TContext : DbContext, new()
    {
        private readonly TContext _entities = new TContext();
        public TContext Context
        {

            get { return _entities; }
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            IQueryable<TEntity> query = _entities.Set<TEntity>();
            return query;
        }

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            IQueryable<TEntity> query = _entities.Set<TEntity>().Where(predicate);
            return query;
        }

        public virtual void Add(TEntity entity)
        {
            _entities.Set<TEntity>().Add(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            _entities.Set<TEntity>().Remove(entity);
        }

        public virtual void Edit(TEntity entity)
        {
            _entities.Entry(entity).State = System.Data.EntityState.Modified;
        }

        public virtual void SaveChanges()
        {
            _entities.SaveChanges();
        }
    }
}