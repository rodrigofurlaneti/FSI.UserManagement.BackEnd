using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _memoryDb;
        private readonly AppDbContextMySql _mysqlDb;

        public UserRepository(AppDbContext memoryDb, AppDbContextMySql mysqlDb)
        {
            _memoryDb = memoryDb;
            _mysqlDb = mysqlDb;
        }

        public void Insert(User user)
        {
            _memoryDb.Users.Add(user);
            _memoryDb.SaveChanges();

            var nameParam = new MySqlParameter("@p_name", user.Name);
            var emailParam = new MySqlParameter("@p_email", user.Email);
            var passwordParam = new MySqlParameter("@p_passwordHash", user.PasswordHash);
            var idParam = new MySqlParameter("@p_id", user.Id.ToString());
            var createdAtParam = new MySqlParameter("@p_createdAt", user.CreatedAt);

            _mysqlDb.Database.ExecuteSqlRaw(
                "CALL sp_InsertUser(@p_id, @p_name, @p_email, @p_passwordHash, @p_createdAt)",
                idParam, nameParam, emailParam, passwordParam, createdAtParam);
        }

        public bool EmailExists(string email)
        {
            var existsInMemory = _memoryDb.Users.Any(u => u.Email.ToLower() == email.ToLower());
            if (existsInMemory) return true;

            return _mysqlDb.Users.Any(u => u.Email.ToLower() == email.ToLower());
        }

        public IEnumerable<User> GetAll()
        {
            var memoryUsers = _memoryDb.Users.AsNoTracking().ToList();

            if (!memoryUsers.Any())
            {
                return _mysqlDb.Users.AsNoTracking().ToList();
            }

            return memoryUsers;
        }

        public User? GetByEmail(string email)
        {
            var userInMemory = _memoryDb.Users.AsNoTracking()
                .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

            if (userInMemory != null) return userInMemory;

            return _mysqlDb.Users.AsNoTracking()
                .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
        }

        public User? GetById(Guid id)
        {
            var userInMemory = _memoryDb.Users.Find(id);
            if (userInMemory != null) return userInMemory;

            return _mysqlDb.Users.Find(id);
        }

        public void Update(User entity)
        {
            entity.SetUpdated();
            _memoryDb.Users.Update(entity);
            _memoryDb.SaveChanges();

            var idParam = new MySqlParameter("@p_id", entity.Id.ToString());
            var nameParam = new MySqlParameter("@p_name", entity.Name);
            var emailParam = new MySqlParameter("@p_email", entity.Email);
            var passwordParam = new MySqlParameter("@p_passwordHash", entity.PasswordHash);
            var updatedAtParam = new MySqlParameter("@p_updatedAt", entity.UpdatedAt);

            _mysqlDb.Database.ExecuteSqlRaw(
                "CALL sp_UpdateUser(@p_id, @p_name, @p_email, @p_passwordHash, @p_updatedAt)",
                idParam, nameParam, emailParam, passwordParam, updatedAtParam);
        }

        public void Delete(Guid id)
        {
            var userInMemory = _memoryDb.Users.Find(id);
            if (userInMemory != null)
            {
                _memoryDb.Users.Remove(userInMemory);
                _memoryDb.SaveChanges();
            }

            var idParam = new MySqlParameter("@p_id", id.ToString());
            _mysqlDb.Database.ExecuteSqlRaw("CALL sp_DeleteUser(@p_id)", idParam);
        }

        public bool Exists(Expression<Func<User, bool>> predicate)
        {
            var existsInMemory = _memoryDb.Users.AsNoTracking().Any(predicate);
            if (existsInMemory) return true;

            return _mysqlDb.Users.AsNoTracking().Any(predicate);
        }

        public IEnumerable<User> Find(Expression<Func<User, bool>> predicate)
        {
            var memoryResults = _memoryDb.Users.AsNoTracking().Where(predicate).ToList();

            if (memoryResults.Any()) return memoryResults;

            return _mysqlDb.Users.AsNoTracking().Where(predicate).ToList();
        }

        public PaginatedResult<User> GetPaged(int page, int pageSize, Expression<Func<User, bool>>? predicate = null)
        {
            var query = _mysqlDb.Users.AsQueryable();

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