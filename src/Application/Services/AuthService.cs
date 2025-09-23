using Domain.Configuration;
using Microsoft.Extensions.Options;

namespace Application.Services
{
    public class AuthService
    {
        private readonly DevelopmentUserOptions _devUser;

        public AuthService(IOptions<DevelopmentUserOptions> devUserOptions)
        {
            _devUser = devUserOptions.Value;
        }

        public bool Authenticate(string username, string password)
        {
            // Validação simples contra o usuário definido no appsettings.json
            return username == _devUser.Username && password == _devUser.Password;
        }
    }
}