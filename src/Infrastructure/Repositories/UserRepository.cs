using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public void Insert(User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public bool EmailExists(string email)
        {
            return _db.Users.Any(u => u.Email == email);
        }

        public IEnumerable<User> GetAll()
        {
            return _db.Users.AsNoTracking().ToList();
        }

        public User? GetByEmail(string email)
        {
            return _db.Users.AsNoTracking().FirstOrDefault(u => u.Email == email);
        }

        public User? GetById(Guid id)
        {
            return _db.Users.Find(id);
        }

        public void Update(User entity)
        {
            _db.Users.Update(entity);
            _db.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var user = _db.Users.Find(id);
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
            }
        }

        public bool Exists(Expression<Func<User, bool>> predicate)
        {
            return _db.Users.Any(predicate);
        }

        public IEnumerable<User> Find(Expression<Func<User, bool>> predicate)
        {
            return _db.Users.AsNoTracking().Where(predicate).ToList();
        }

        public PaginatedResult<User> GetPaged(int page, int pageSize, Expression<Func<User, bool>>? predicate = null)
        {
            var query = _db.Users.AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);

            var totalCount = query.LongCount();

            var items = query
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResult<User>(
                Items: items,
                Page: page,
                PageSize: pageSize,
                TotalCount: totalCount
            );
        }
    }
}