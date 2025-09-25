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
            // 1. Insert no MySQL primeiro
            var nameParam = new MySqlParameter("@p_name", user.Name);
            var emailParam = new MySqlParameter("@p_email", user.Email);
            var passwordParam = new MySqlParameter("@p_passwordHash", user.PasswordHash);
            var idParam = new MySqlParameter("@p_id", user.Id.ToString());
            var createdAtParam = new MySqlParameter("@p_createdAt", user.CreatedAt);

            _mysqlDb.Database.ExecuteSqlRaw(
                "CALL sp_InsertUser(@p_id, @p_name, @p_email, @p_passwordHash, @p_createdAt)",
                idParam, nameParam, emailParam, passwordParam, createdAtParam);

            // 2. Atualiza o cache
            _memoryDb.Users.Add(user);
            _memoryDb.SaveChanges();
        }

        public bool EmailExists(string email)
        {
            // Sincroniza cache antes de verificar
            SyncCacheIfNeeded();

            return _memoryDb.Users.Any(u => u.Email.ToLower() == email.ToLower());
        }

        public IEnumerable<User> GetAll()
        {
            // Sincroniza cache antes de retornar
            SyncCacheIfNeeded();

            return _memoryDb.Users.AsNoTracking().ToList();
        }

        public User? GetByEmail(string email)
        {
            // Sincroniza cache antes de buscar
            SyncCacheIfNeeded();

            return _memoryDb.Users.AsNoTracking()
                .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
        }

        public User? GetById(Guid id)
        {
            // Sincroniza cache antes de buscar
            SyncCacheIfNeeded();

            return _memoryDb.Users.Find(id);
        }

        public void Update(User entity)
        {
            entity.SetUpdated();

            // 1. Update no MySQL primeiro
            var idParam = new MySqlParameter("@p_id", entity.Id.ToString());
            var nameParam = new MySqlParameter("@p_name", entity.Name);
            var emailParam = new MySqlParameter("@p_email", entity.Email);
            var passwordParam = new MySqlParameter("@p_passwordHash", entity.PasswordHash);
            var updatedAtParam = new MySqlParameter("@p_updatedAt", entity.UpdatedAt);

            _mysqlDb.Database.ExecuteSqlRaw(
                "CALL sp_UpdateUser(@p_id, @p_name, @p_email, @p_passwordHash, @p_updatedAt)",
                idParam, nameParam, emailParam, passwordParam, updatedAtParam);

            // 2. Atualiza no cache
            var cachedUser = _memoryDb.Users.Find(entity.Id);
            if (cachedUser != null)
            {
                _memoryDb.Entry(cachedUser).CurrentValues.SetValues(entity);
            }
            else
            {
                _memoryDb.Users.Add(entity);
            }
            _memoryDb.SaveChanges();
        }

        public void Delete(Guid id)
        {
            // 1. Delete no MySQL primeiro
            var idParam = new MySqlParameter("@p_id", id.ToString());
            _mysqlDb.Database.ExecuteSqlRaw("CALL sp_DeleteUser(@p_id)", idParam);

            // 2. Remove do cache
            var userInMemory = _memoryDb.Users.Find(id);
            if (userInMemory != null)
            {
                _memoryDb.Users.Remove(userInMemory);
                _memoryDb.SaveChanges();
            }
        }

        public bool Exists(Expression<Func<User, bool>> predicate)
        {
            // Sincroniza cache antes de verificar
            SyncCacheIfNeeded();

            return _memoryDb.Users.AsNoTracking().Any(predicate);
        }

        public IEnumerable<User> Find(Expression<Func<User, bool>> predicate)
        {
            // Sincroniza cache antes de buscar
            SyncCacheIfNeeded();

            return _memoryDb.Users.AsNoTracking().Where(predicate).ToList();
        }

        public PaginatedResult<User> GetPaged(int page, int pageSize, Expression<Func<User, bool>>? predicate = null)
        {
            // Para paginação, usa diretamente o MySQL para performance
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

        private void SyncCacheIfNeeded()
        {
            // Verifica se o cache está vazio ou desatualizado
            var cacheCount = _memoryDb.Users.Count();
            var dbCount = _mysqlDb.Users.Count();

            if (cacheCount == 0 || cacheCount != dbCount)
            {
                // Limpa o cache atual
                _memoryDb.Users.RemoveRange(_memoryDb.Users);

                // Carrega todos os dados do MySQL para o cache
                var allUsers = _mysqlDb.Users.AsNoTracking().ToList();
                _memoryDb.Users.AddRange(allUsers);
                _memoryDb.SaveChanges();
            }
        }
    }
}