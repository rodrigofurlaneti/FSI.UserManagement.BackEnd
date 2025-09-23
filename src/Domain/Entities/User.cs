using Domain.Shared;
using System;

namespace Domain.Entities
{
    public class User : BaseEntity
    {
        #region Properties

        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }

        #endregion

        #region Constructors 

        private User() { }

        public User(string name, string email, string passwordHash)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        }

        #endregion

        #region Methods

        public void UpdatePasswordHash(string newHash)
        {
            PasswordHash = newHash ?? throw new ArgumentNullException(nameof(newHash));
        }

        #endregion
    }
}
