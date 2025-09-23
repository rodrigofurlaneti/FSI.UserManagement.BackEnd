using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Domain.Shared;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class EfRepositorySync<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _db;
        protected readonly DbSet<T> _set;

        public EfRepositorySync(AppDbContext db)
        {
            _db = db;
            _set = db.Set<T>();
        }

        public virtual void Insert(T entity)
        {
            _set.Add(entity);
            _db.SaveChanges();
        }

        public virtual void Delete(Guid id)
        {
            var entity = _set.Find(id);
            if (entity == null) return;
            entity.MarkDeleted();
            entity.SetUpdated();
            _set.Update(entity);
            _db.SaveChanges();
        }

        public virtual bool Exists(Expression<Func<T, bool>> predicate)
        {
            return _set.AsNoTracking().Any(predicate);
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _set.AsNoTracking().Where(predicate).ToList();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _set.AsNoTracking().ToList();
        }

        public virtual T? GetById(Guid id)
        {
            return _set.Find(id);
        }

        public virtual void Update(T entity)
        {
            entity.SetUpdated();
            _set.Update(entity);
            _db.SaveChanges();
        }

        public virtual PaginatedResult<T> GetPaged(int page, int pageSize, Expression<Func<T, bool>>? predicate = null)
        {
            var query = _set.AsNoTracking().AsQueryable();
            if (predicate != null) query = query.Where(predicate);
            var total = query.LongCount();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedResult<T>(items, page, pageSize, total);
        }
    }
}