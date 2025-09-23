using System;
using System.Collections.Generic;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepositorySync : EfRepositorySync<User>, IRepository<User>
    {
        public UserRepositorySync(AppDbContext db) : base(db) { }

        public User? GetByEmail(string email)
        {
            return _set.AsNoTracking().FirstOrDefault(u => u.Email == email);
        }

        public bool EmailExists(string email)
        {
            return _set.AsNoTracking().Any(u => u.Email == email);
        }
    }
}
