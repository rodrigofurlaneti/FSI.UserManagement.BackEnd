using Domain.Entities;

namespace Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        User? GetByEmail(string email);
        bool EmailExists(string email);
    }
}
