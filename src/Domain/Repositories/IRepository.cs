using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IRepository<TEntity>
    {
        void Insert(TEntity entity);
        void Delete(Guid id);
        TEntity? GetById(Guid id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        bool Exists(Expression<Func<TEntity, bool>> predicate);
        void Update(TEntity entity);
        PaginatedResult<TEntity> GetPaged(int page, int pageSize, Expression<Func<TEntity, bool>>? predicate = null);
    }
}
